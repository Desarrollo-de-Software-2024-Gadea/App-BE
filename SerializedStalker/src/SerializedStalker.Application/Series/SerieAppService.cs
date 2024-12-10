using Microsoft.AspNetCore.Authorization;
using SerializedStalker.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Volo.Abp.ObjectMapping;
using Microsoft.Extensions.Logging;


namespace SerializedStalker.Series
{
    //[Authorize]
    public class SerieAppService : CrudAppService<Serie, SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>, ISerieAppService
    {
        private readonly ISeriesApiService _seriesApiService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IObjectMapper _objectMapper;
        private readonly IMonitoreoApiAppService _monitoreoApiAppService;
        private readonly ILogger<SerieAppService> _logger;

        public SerieAppService(
            IRepository<Serie, int> repository,
            ISeriesApiService seriesApiService,
            ICurrentUserService currentUserService,
            IObjectMapper objectMapper,
            IMonitoreoApiAppService monitoreoApiAppService,
            ILogger<SerieAppService> logger)
        : base(repository)
        {
            _seriesApiService = seriesApiService;
            _serieRepository = repository;
            _currentUserService = currentUserService;
            _objectMapper = objectMapper;
            _monitoreoApiAppService = monitoreoApiAppService;
            _logger = logger;
        }

        // Métodos de búsqueda

        /// <summary>
        /// Busca series por título y opcionalmente por género utilizando el servicio de API de series.
        /// </summary>
        /// <param name="titulo">El título de la serie a buscar.</param>
        /// <param name="genero">El género de la serie a buscar (opcional).</param>
        /// <returns>Un array de objetos SerieDto que coinciden con los criterios de búsqueda.</returns>
        /// <exception cref="Exception">Lanzada si ocurre un error durante la búsqueda de series.</exception>
        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero = null)
        {
            var monitoreo = new MonitoreoApiDto
            {
                HoraEntrada = DateTime.Now
            };

            try
            {
                var series = await _seriesApiService.BuscarSerieAsync(titulo, genero);
                monitoreo.HoraSalida = DateTime.Now;
                monitoreo.TiempoDuracion = (float)(monitoreo.HoraSalida - monitoreo.HoraEntrada).TotalSeconds;
                await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreo);
                _logger.LogInformation("Búsqueda de series completada correctamente.");
                return series;
            }
            catch (Exception ex)
            {
                monitoreo.HoraSalida = DateTime.Now;
                monitoreo.TiempoDuracion = (float)(monitoreo.HoraSalida - monitoreo.HoraEntrada).TotalSeconds;
                monitoreo.Errores.Add("Excepción: " + ex.Message);
                await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreo);
                _logger.LogError(ex, "Error al buscar series.");
                throw;
            }
        }

        /// <summary>
        /// Busca una temporada específica de una serie utilizando el identificador IMDb y el número de temporada.
        /// </summary>
        /// <param name="imdbId">El identificador IMDb de la serie.</param>
        /// <param name="numeroTemporada">El número de la temporada a buscar.</param>
        /// <returns>Un objeto TemporadaDto que representa la temporada buscada.</returns>
        /// <exception cref="Exception">Lanzada si ocurre un error durante la búsqueda de la temporada.</exception>
        public async Task<TemporadaDto> BuscarTemporadaAsync(string imdbId, int numeroTemporada)
        {
            var monitoreo = new MonitoreoApiDto
            {
                HoraEntrada = DateTime.Now
            };

            try
            {
                var temporada = await _seriesApiService.BuscarTemporadaAsync(imdbId, numeroTemporada);
                monitoreo.HoraSalida = DateTime.Now;
                monitoreo.TiempoDuracion = (float)(monitoreo.HoraSalida - monitoreo.HoraEntrada).TotalSeconds;
                await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreo);
                _logger.LogInformation("Búsqueda de temporada completada correctamente.");
                return temporada;
            }
            catch (Exception ex)
            {
                monitoreo.HoraSalida = DateTime.Now;
                monitoreo.TiempoDuracion = (float)(monitoreo.HoraSalida - monitoreo.HoraEntrada).TotalSeconds;
                monitoreo.Errores.Add("Excepción: " + ex.Message);
                await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreo);
                _logger.LogError(ex, "Error al buscar temporada.");
                throw;
            }
        }

        // Métodos de calificación

        /// <summary>
        /// Permite a un usuario calificar una serie.
        /// </summary>
        /// <param name="input">Un objeto CalificacionDto que contiene la calificación y el comentario del usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="EntityNotFoundException">Lanzada si la serie no se encuentra en el repositorio.</exception>
        /// <exception cref="InvalidOperationException">Lanzada si el ID del usuario actual es nulo o si el usuario ya ha calificado la serie.</exception>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario intenta calificar una serie que no le pertenece.</exception>
        public async Task CalificarSerieAsync(CalificacionDto input)
        {
            try
            {
                var serie = await _serieRepository.GetAsync(input.SerieID);
                if (serie == null)
                {
                    throw new EntityNotFoundException(typeof(Serie), input.SerieID);
                }

                var userIdActual = _currentUserService.GetCurrentUserId();
                if (!userIdActual.HasValue)
                {
                    throw new InvalidOperationException("User ID cannot be null");
                }

                if (serie.CreatorId != userIdActual.Value)
                {
                    throw new UnauthorizedAccessException("No puedes calificar esta serie.");
                }

                var calificacionExistente = serie.Calificaciones.FirstOrDefault(c => c.UsuarioId == userIdActual.Value);
                if (calificacionExistente != null)
                {
                    throw new InvalidOperationException("Ya has calificado esta serie.");
                }

                var calificacion = new Calificacion
                {
                    NroCalificacion = input.NroCalificacion,
                    Comentario = input.Comentario,
                    FechaCreacion = DateTime.Now,
                    SerieID = input.SerieID,
                    UsuarioId = userIdActual.Value
                };

                serie.Calificaciones.Add(calificacion);
                await _serieRepository.UpdateAsync(serie);
                _logger.LogInformation("Serie calificada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calificar la serie.");
                throw;
            }
        }

        /// <summary>
        /// Permite a un usuario modificar su calificación existente de una serie.
        /// </summary>
        /// <param name="input">Un objeto CalificacionDto que contiene la nueva calificación y el comentario del usuario.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="ArgumentNullException">Lanzada si el objeto input es nulo.</exception>
        /// <exception cref="EntityNotFoundException">Lanzada si la serie no se encuentra en el repositorio.</exception>
        /// <exception cref="InvalidOperationException">Lanzada si el ID del usuario actual es nulo o si no hay una calificación existente para modificar.</exception>
        /// <exception cref="UnauthorizedAccessException">Lanzada si el usuario intenta modificar una calificación de una serie que no le pertenece.</exception>
        public async Task ModificarCalificacionAsync(CalificacionDto input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            try
            {
                var serie = await _serieRepository.GetAsync(input.SerieID);
                if (serie == null)
                {
                    throw new EntityNotFoundException(typeof(Serie), input.SerieID);
                }

                var userIdActual = _currentUserService.GetCurrentUserId();
                if (!userIdActual.HasValue)
                {
                    throw new InvalidOperationException("User ID cannot be null");
                }

                if (serie.CreatorId != userIdActual.Value)
                {
                    throw new UnauthorizedAccessException("No puedes calificar esta serie.");
                }

                var calificacionExistente = serie.Calificaciones.FirstOrDefault(c => c.UsuarioId == userIdActual.Value);
                if (calificacionExistente == null)
                {
                    throw new InvalidOperationException("No hay calificación que modificar.");
                }

                calificacionExistente.NroCalificacion = input.NroCalificacion;
                calificacionExistente.Comentario = input.Comentario;
                calificacionExistente.FechaCreacion = DateTime.Now;

                await _serieRepository.UpdateAsync(serie);
                _logger.LogInformation("Calificación modificada correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al modificar la calificación.");
                throw;
            }
        }

        // Métodos de persistencia

        /// <summary>
        /// Persiste una lista de series en la base de datos.
        /// </summary>
        /// <param name="seriesDto">Un array de objetos SerieDto que representan las series a persistir.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="InvalidOperationException">Lanzada si el ID del usuario actual es nulo o si el identificador IMDb de una serie es nulo.</exception>
        public async Task PersistirSeriesAsync(SerieDto[] seriesDto)
        {
            try
            {
                var seriesExistentes = await _serieRepository.GetListAsync();

                if (seriesExistentes == null)
                {
                    seriesExistentes = new List<Serie>();
                }

                 foreach (var serieDto in seriesDto)
                {
                    if (serieDto == null) continue;

                    /*var userIdActual = _currentUserService.GetCurrentUserId();
                    if (!userIdActual.HasValue)
                    {
                        throw new InvalidOperationException("User ID cannot be null");
                    }*/

                    if (serieDto.ImdbIdentificator == null)
                    {
                        throw new InvalidOperationException("ImdbIdentificator cannot be null");
                    }

                    var serieExistente = seriesExistentes.FirstOrDefault(s => s.ImdbIdentificator == serieDto.ImdbIdentificator/* && s.CreatorId == userIdActual.Value*/);

                    if (serieExistente == null)
                    {
                        var nuevaSerie = _objectMapper.Map<SerieDto, Serie>(serieDto);
                        nuevaSerie.Temporadas = new List<Temporada>();
                        //nuevaSerie.CreatorId = userIdActual.Value;

                        if (serieDto.Temporadas != null)
                        {
                            foreach (var temporadaDto in serieDto.Temporadas)
                            {
                                var nuevaTemporada = _objectMapper.Map<TemporadaDto, Temporada>(temporadaDto);
                                nuevaSerie.Temporadas.Add(nuevaTemporada);
                            }
                        }

                        await _serieRepository.InsertAsync(nuevaSerie);
                    }
                    else
                    {
                        if (serieExistente.TotalTemporadas == serieDto.TotalTemporadas)
                        {
                            throw new InvalidOperationException("Serie ya esta persistida");
                        }
                        else
                        {
                            serieExistente.TotalTemporadas = serieDto.TotalTemporadas;
                            if (serieDto.Temporadas != null)
                            {
                                foreach (var temporadaDto in serieDto.Temporadas)
                                {
                                    var temporadaExistente = serieExistente.Temporadas.FirstOrDefault(t => t.NumeroTemporada == temporadaDto.NumeroTemporada);
                                    if (temporadaExistente == null)
                                    {
                                        var nuevaTemporada = _objectMapper.Map<TemporadaDto, Temporada>(temporadaDto);
                                        serieExistente.Temporadas.Add(nuevaTemporada);
                                    }
                                    else
                                    {
                                        _objectMapper.Map(temporadaDto, temporadaExistente);
                                    }
                                }
                            }

                            await _serieRepository.UpdateAsync(serieExistente);
                        }
                    }
                }
                _logger.LogInformation("Series persistidas correctamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al persistir las series.");
                throw;
            }
        }

        /// <summary>
        /// Obtiene todas las series almacenadas en el repositorio.
        /// </summary>
        /// <returns>Un arreglo de objetos <see cref="SerieDto"/> que representan las series almacenadas.</returns>
        /// <exception cref="Exception">Lanzada cuando no hay series persistidas en el repositorio.</exception>
        public async Task<SerieDto[]> ObtenerSeriesAsync()
        {
            try
            {
                var series = await _serieRepository.GetListAsync();

                if (series == null)
                {
                    throw new Exception("No hay series Persistidas.");
                }

                _logger.LogInformation("Series obtenidas correctamente.");
                return _objectMapper.Map<Serie[], SerieDto[]>(series.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener las series.");
                throw;
            }
        }
    }
}

