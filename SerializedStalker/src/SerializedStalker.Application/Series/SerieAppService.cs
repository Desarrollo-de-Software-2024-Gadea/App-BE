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


        public SerieAppService(IRepository<Serie, int> repository, ISeriesApiService seriesApiService, ICurrentUserService currentUserService,
            IObjectMapper objectMapper, IMonitoreoApiAppService monitoreoApiAppService)
        : base(repository)
        {
            _seriesApiService = seriesApiService;
            _serieRepository = repository;
            _currentUserService = currentUserService;
            _objectMapper = objectMapper;
            _monitoreoApiAppService = monitoreoApiAppService;
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
                return series;
            }
            catch (Exception ex)
            {
                monitoreo.HoraSalida = DateTime.Now;
                monitoreo.TiempoDuracion = (float)(monitoreo.HoraSalida - monitoreo.HoraEntrada).TotalSeconds;
                monitoreo.Errores.Add("Excepción: " + ex.Message);
                await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreo);
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
                return temporada;
            }
            catch (Exception ex)
            {
                monitoreo.HoraSalida = DateTime.Now;
                monitoreo.TiempoDuracion = (float)(monitoreo.HoraSalida - monitoreo.HoraEntrada).TotalSeconds;
                monitoreo.Errores.Add("Excepción: " + ex.Message);
                await _monitoreoApiAppService.PersistirMonitoreoAsync(monitoreo);
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
            // Obtener la serie del repositorio
            var serie = await _serieRepository.GetAsync(input.SerieID);
            if (serie == null)
            {
                throw new EntityNotFoundException(typeof(Serie), input.SerieID);
            }

            // Obtener el ID del usuario actual
            var userIdActual = _currentUserService.GetCurrentUserId();
            if (!userIdActual.HasValue)
            {
                throw new InvalidOperationException("User ID cannot be null");
            }

            // Un usuario solo puede calificar las series relacionadas a él
            if (serie.CreatorId != userIdActual.Value)
            {
                throw new UnauthorizedAccessException("No puedes calificar esta serie.");
            }

            // Un usuario no puede calificar 2 veces la misma serie
            var calificacionExistente = serie.Calificaciones.FirstOrDefault(c => c.UsuarioId == userIdActual.Value);
            if (calificacionExistente != null)
            {
                throw new InvalidOperationException("Ya has calificado esta serie.");
            }

            // Crear la nueva calificación
            var calificacion = new Calificacion
            {
                calificacion = input.calificacion,
                comentario = input.comentario,
                FechaCreacion = DateTime.Now, // Asegúrate de asignar la fecha de creación
                SerieID = input.SerieID,
                UsuarioId = userIdActual.Value // Asigna el ID del usuario actual
            };

            // Agregar la calificación a la serie
            serie.Calificaciones.Add(calificacion);

            // Actualizar la serie en el repositorio
            await _serieRepository.UpdateAsync(serie);
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

            // Obtener la serie del repositorio
            var serie = await _serieRepository.GetAsync(input.SerieID);
            if (serie == null)
            {
                throw new EntityNotFoundException(typeof(Serie), input.SerieID);
            }

            // Obtener el ID del usuario actual
            var userIdActual = _currentUserService.GetCurrentUserId();
            if (!userIdActual.HasValue)
            {
                throw new InvalidOperationException("User ID cannot be null");
            }

            // Un usuario solo puede calificar las series relacionadas a él
            if (serie.CreatorId != userIdActual.Value)
            {
                throw new UnauthorizedAccessException("No puedes calificar esta serie.");
            }

            // Un usuario no puede modificar una calificación que no existe
            var calificacionExistente = serie.Calificaciones.FirstOrDefault(c => c.UsuarioId == userIdActual.Value);
            if (calificacionExistente == null)
            {
                throw new InvalidOperationException("No hay calificación que modificar.");
            }


            // Crear una nueva instancia o modificar la existente
            calificacionExistente.calificacion = input.calificacion; // Asegúrate de usar la propiedad correcta de input
            calificacionExistente.comentario = input.comentario;
            calificacionExistente.FechaCreacion = DateTime.Now; // Puedes agregar esta propiedad para auditar cambios        

            // Actualizar la serie en el repositorio
            await _serieRepository.UpdateAsync(serie);
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
            // Obtener todas las series existentes
            var seriesExistentes = await _serieRepository.GetListAsync();

            if (seriesExistentes == null)
            {
                seriesExistentes = new List<Serie>();
            }

            foreach (var serieDto in seriesDto)
            {
                // Comprobación para evitar excepciones al acceder a propiedades de un objeto que podría ser null
                if (serieDto == null) continue; // Salta si serieDto es null

                var userIdActual = _currentUserService.GetCurrentUserId();
                if (!userIdActual.HasValue)
                {
                    throw new InvalidOperationException("User ID cannot be null");
                }

                // Verificar que serieDto.ImdbIdentificator no sea null
                if (serieDto.ImdbIdentificator == null)
                {
                    throw new InvalidOperationException("ImdbIdentificator cannot be null");
                }

                var serieExistente = seriesExistentes.FirstOrDefault(s => s.ImdbIdentificator == serieDto.ImdbIdentificator && s.CreatorId == userIdActual.Value);

                if (serieExistente == null)
                {
                    // Crear nueva serie
                    var nuevaSerie = _objectMapper.Map<SerieDto, Serie>(serieDto);
                    nuevaSerie.CreatorId = userIdActual.Value; // Asignar el ID del creador

                    // Asegúrate de que Temporadas no sea null
                    if (serieDto.Temporadas != null)
                    {
                        foreach (var temporadaDto in serieDto.Temporadas)
                        {
                            var nuevaTemporada = _objectMapper.Map<TemporadaDto, Temporada>(temporadaDto);
                            nuevaSerie.Temporadas.Add(nuevaTemporada);
                        }
                    }

                    // Persistir la nueva serie en la base de datos
                    await _serieRepository.InsertAsync(nuevaSerie);
                }
                else
                {
                    // Actualizar la serie existente con nueva información
                    serieExistente.TotalTemporadas = serieDto.TotalTemporadas;

                    // Actualizar temporadas si es necesario
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
                                // Actualizar la temporada existente
                                _objectMapper.Map(temporadaDto, temporadaExistente);
                            }
                        }
                    }

                    await _serieRepository.UpdateAsync(serieExistente);
                }
            }
        }

        /// <summary>
        /// Obtiene todas las series almacenadas en el repositorio.
        /// </summary>
        /// <returns>Un arreglo de objetos <see cref="SerieDto"/> que representan las series almacenadas.</returns>
        /// <exception cref="Exception">Lanzada cuando no hay series persistidas en el repositorio.</exception>
        public async Task<SerieDto[]> ObtenerSeriesAsync()
        {
            //Obtenemos las series del Repositorio
            var series = await _serieRepository.GetListAsync();

            // Si no hay series da una excepción
            if (series == null)
            {
                throw new Exception("No hay series Persistidas.");
            }

            return _objectMapper.Map<Serie[], SerieDto[]>(series.ToArray());
        }
    }
}
