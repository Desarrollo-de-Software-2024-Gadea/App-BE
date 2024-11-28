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

namespace SerializedStalker.Series
{
    [Authorize]
    public class SerieAppService : CrudAppService<Serie, SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>, ISerieAppService
    {
        private readonly ISeriesApiService _seriesApiService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUserService _currentUserService;

        public SerieAppService(IRepository<Serie, int> repository, ISeriesApiService seriesApiService, ICurrentUserService currentUserService)
        : base(repository)
        {
            _seriesApiService = seriesApiService;
            _serieRepository = repository;
            _currentUserService = currentUserService;
        }

        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero = null)
        {
            return await _seriesApiService.BuscarSerieAsync(titulo, genero);
        }

        // Nuevo método para buscar temporadas
        public async Task<TemporadaDto> BuscarTemporadaAsync(string imdbId, int numeroTemporada)
        {
            return await _seriesApiService.BuscarTemporadaAsync(imdbId, numeroTemporada);
        }

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
    }
}
