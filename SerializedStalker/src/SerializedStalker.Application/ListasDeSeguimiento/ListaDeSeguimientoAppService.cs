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


namespace SerializedStalker.ListasDeSeguimiento
{
    [Authorize]
    public class ListaDeSeguimientoAppService : ApplicationService, IListaDeSeguimientoAppService
    {
        private readonly IRepository<ListaDeSeguimiento,int> _listaDeSeguimientoRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;
        private readonly OmdbService _service;
        private readonly SerieAppService _serieAppService;
        private readonly IObjectMapper _objectMapper;

        public ListaDeSeguimientoAppService(IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository,
            IRepository<Serie, int> serieRepository, ICurrentUser currentUser, IObjectMapper objectMapper, OmdbService service, SerieAppService serieAppService)
        { 
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
            _serieRepository = serieRepository;
            _currentUser = currentUser;
            _objectMapper = objectMapper;
            _service = service;
            _serieAppService = serieAppService;
        }
        public async Task AddSerieAsync(string titulo)
        {
            //Debemos agregar una forma de que se extraiga solo la lista del usuario actual.
            Guid userId = (Guid)_currentUser.Id;
            // Obtén la lista de seguimiento, asumiendo que solo hay una por ahora
            var listaDeSeguimiento = (await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault(l => l.CreatorId == userId);

            // Si no existe, crea una nueva lista de seguimiento
            if (listaDeSeguimiento == null)
            {
                listaDeSeguimiento = new ListaDeSeguimiento()
                {
                    FechaModificacion = DateOnly.FromDateTime(DateTime.Now),
                };
                await _listaDeSeguimientoRepository.InsertAsync(listaDeSeguimiento);
            }

            //Obtenemos el serieDto
            var serieApi = await _service.BuscarSerieAsync(titulo, null);
            // Busca la serie por ID
            //var serie = await _serieRepository.GetAsync(serieID);

            // Comprueba si la serie ya está en la lista
            if (!listaDeSeguimiento.Series.Any(s => s.ImdbIdentificator == serieApi.FirstOrDefault().ImdbIdentificator))
            {
                await _serieAppService.PersistirSeriesAsync(serieApi);
                var serie = (await _serieRepository.GetListAsync()).LastOrDefault();
                listaDeSeguimiento.Series.Add(serie); // Añade la serie a la lista
                listaDeSeguimiento.FechaModificacion = DateOnly.FromDateTime(DateTime.Now); //Actualiza la fecha de modificación
            }
            else
            {
                throw new Exception("La serie ya está en la lista de seguimiento."); // Maneja el caso en que la serie ya está en la lista
            }

            // Actualiza la lista de seguimiento en la base de datos
            await _listaDeSeguimientoRepository.UpdateAsync(listaDeSeguimiento);
         }
        public async Task<SerieDto[]> MostrarSeriesAsync()
        {
            //var userEnt = _currentUser;
            Guid userId = (Guid)_currentUser.Id;
            // Obtén la lista de seguimiento, asumiendo que solo hay una por ahora
            var listaDeSeguimiento = (await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault(l => l.CreatorId == userId);

            // Si no existe, crea una nueva lista de seguimiento
            if (listaDeSeguimiento == null)
            {
                throw new Exception("No hay serie que mostrar.");
            }
            else
            {
                var listaSeries = new List<SerieDto>();
                var series = listaDeSeguimiento.Series.ToArray();
                foreach (var serie in series)
                {
                    var elementoLista = _objectMapper.Map<Serie, SerieDto>(serie);
                    listaSeries.Add(elementoLista);
                }
                return listaSeries.ToArray();
            }
        }
        public async Task EliminarSerieAsync(string ImdbID)
        {
            //var userEnt = _currentUser;
            Guid userId = (Guid)_currentUser.Id;
            // Obtén la lista de seguimiento, asumiendo que solo hay una por ahora
            var listaDeSeguimiento = (await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault(l => l.CreatorId == userId);

            // Si no existe, crea una nueva lista de seguimiento
            if (listaDeSeguimiento == null)
            {
                throw new Exception("No existe Lista de seguimiento.");
            }
            if (listaDeSeguimiento.Series.Any(s => s.ImdbIdentificator == ImdbID))
            {
                var serie = (await _serieRepository.GetListAsync()).LastOrDefault();
                listaDeSeguimiento.Series.Remove(serie); // Saca la serie a la lista
                await _serieRepository.DeleteAsync(serie);
                listaDeSeguimiento.FechaModificacion = DateOnly.FromDateTime(DateTime.Now); //Actualiza la fecha de modificación
            }
            else
            {
                throw new Exception("No hay serie que eliminar.");
            }
            // Actualiza la lista de seguimiento en la base de datos
            await _listaDeSeguimientoRepository.UpdateAsync(listaDeSeguimiento);

        }
    }
}

/*
Como la idea del profe es que las series se persistan por separado, 
debemos buscar la serie en la api y ahí persistirla, para luego agregarla a la lista de seguimiento.
Debemos tambien tener en cuenta el usuario que la agrega a su lista así lo ponemos en la serie.
*/