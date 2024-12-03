using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Microsoft.AspNetCore.Authorization;
using Volo.Abp.ObjectMapping;
using Microsoft.Extensions.Logging;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;
using static Volo.Abp.Identity.IdentityPermissions;

namespace SerializedStalker.ListasDeSeguimiento
{
    //[Authorize]
    public class ListaDeSeguimientoAppService : ApplicationService, IListaDeSeguimientoAppService
    {
        private readonly IRepository<ListaDeSeguimiento, int> _listaDeSeguimientoRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;
        private readonly OmdbService _service;
        private readonly SerieAppService _serieAppService;
        private readonly IObjectMapper _objectMapper;
        private readonly ILogger<ListaDeSeguimientoAppService> _logger;

        public ListaDeSeguimientoAppService(
            IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository,
            IRepository<Serie, int> serieRepository,
            ICurrentUser currentUser,
            IObjectMapper objectMapper,
            OmdbService service,
            SerieAppService serieAppService,
            ILogger<ListaDeSeguimientoAppService> logger)
        {
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
            _serieRepository = serieRepository;
            _currentUser = currentUser;
            _objectMapper = objectMapper;
            _service = service;
            _serieAppService = serieAppService;
            _logger = logger;
        }

        /// <summary>
        /// Agrega una serie a la lista de seguimiento del usuario actual.
        /// </summary>
        /// <param name="serieDto">El objeto SerieDto que contiene la información de la serie a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="Exception">Se lanza una excepción si la serie ya está en la lista de seguimiento.</exception>
        public async Task AddSerieAsync(SerieDto serieDto)
        {
            _logger.LogInformation("Iniciando AddSerieAsync para el usuario {UserId}", 0);//_currentUser.Id);

            Guid userId = (Guid)_currentUser.Id;
            //var listaDeSeguimiento = (await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault(l => l.CreatorId == userId);
            var queryable = await _listaDeSeguimientoRepository.WithDetailsAsync(x => x.Series);
            var listaDeSeguimiento = await AsyncExecuter.FirstOrDefaultAsync(queryable);
            if (listaDeSeguimiento == null)
            {
                listaDeSeguimiento = new ListaDeSeguimiento()
                {
                    FechaModificacion = DateOnly.FromDateTime(DateTime.Now),
                };
                await _listaDeSeguimientoRepository.InsertAsync(listaDeSeguimiento);
                _logger.LogInformation("Nueva lista de seguimiento creada para el usuario {UserId}", userId);
            }

            if (!listaDeSeguimiento.Series.Any(s => s.ImdbIdentificator == serieDto.ImdbIdentificator))
            {
                var seriesDto = new List<SerieDto> { serieDto };
                await _serieAppService.PersistirSeriesAsync(seriesDto.ToArray());
                var serie = (await _serieRepository.GetListAsync()).LastOrDefault();
                listaDeSeguimiento.Series.Add(serie);
                listaDeSeguimiento.FechaModificacion = DateOnly.FromDateTime(DateTime.Now);
                _logger.LogInformation("Serie {SerieId} agregada a la lista de seguimiento del usuario {UserId}", serie.Id, userId);
            }
            else
            {
                _logger.LogWarning("La serie {SerieId} ya está en la lista de seguimiento del usuario {UserId}", serieDto.Id, userId);
                throw new Exception("La serie ya está en la lista de seguimiento.");
            }

            await _listaDeSeguimientoRepository.UpdateAsync(listaDeSeguimiento);
            _logger.LogInformation("Lista de seguimiento actualizada para el usuario {UserId}", userId);
        }

        /// <summary>
        /// Muestra todas las series en la lista de seguimiento del usuario actual.
        /// </summary>
        /// <returns>Un array de objetos SerieDto que representan las series en la lista de seguimiento.</returns>
        /// <exception cref="Exception">Se lanza una excepción si no hay series en la lista de seguimiento.</exception>
        public async Task<SerieDto[]> MostrarSeriesAsync()
        {
            _logger.LogInformation("Iniciando MostrarSeriesAsync para el usuario {UserId}", _currentUser.Id);

            Guid userId = (Guid)_currentUser.Id;
            var queryable = await _listaDeSeguimientoRepository.WithDetailsAsync(x => x.Series);
            var listaDeSeguimiento = await AsyncExecuter.FirstOrDefaultAsync(queryable);

            if (listaDeSeguimiento == null)
            {
                _logger.LogWarning("No hay series en la lista de seguimiento del usuario {UserId}", userId);
                throw new Exception("No hay serie que mostrar.");
            }

            _logger.LogInformation("Series obtenidas para la lista de seguimiento del usuario {UserId}", userId);
            return ObjectMapper.Map<Serie[], SerieDto[]>(listaDeSeguimiento.Series.ToArray());
        }

        /// <summary>
        /// Elimina una serie de la lista de seguimiento del usuario actual.
        /// </summary>
        /// <param name="ImdbID">El identificador de IMDb de la serie a eliminar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="Exception">Se lanza una excepción si la lista de seguimiento no existe o si la serie no está en la lista de seguimiento.</exception>
        public async Task EliminarSerieAsync(string ImdbID)
        {
            _logger.LogInformation("Iniciando EliminarSerieAsync para el usuario {UserId}", _currentUser.Id);

            Guid userId = (Guid)_currentUser.Id;
            var queryable = await _listaDeSeguimientoRepository.WithDetailsAsync(x => x.Series);
            var listaDeSeguimiento = await AsyncExecuter.FirstOrDefaultAsync(queryable);

            if (listaDeSeguimiento == null)
            {
                _logger.LogWarning("No existe lista de seguimiento para el usuario {UserId}", userId);
                throw new Exception("No existe Lista de seguimiento.");
            }

            if (listaDeSeguimiento.Series.Any(s => s.ImdbIdentificator == ImdbID))
            {
                var serie = (await _serieRepository.GetListAsync()).LastOrDefault();
                listaDeSeguimiento.Series.Remove(serie);
                await _serieRepository.DeleteAsync(serie);
                listaDeSeguimiento.FechaModificacion = DateOnly.FromDateTime(DateTime.Now);
                _logger.LogInformation("Serie {SerieId} eliminada de la lista de seguimiento del usuario {UserId}", serie.Id, userId);
            }
            else
            {
                _logger.LogWarning("No hay serie que eliminar en la lista de seguimiento del usuario {UserId}", userId);
                throw new Exception("No hay serie que eliminar.");
            }

            await _listaDeSeguimientoRepository.UpdateAsync(listaDeSeguimiento);
            _logger.LogInformation("Lista de seguimiento actualizada para el usuario {UserId}", userId);
        }
    }
}