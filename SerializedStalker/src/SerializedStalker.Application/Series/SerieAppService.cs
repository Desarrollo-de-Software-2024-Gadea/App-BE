using Microsoft.AspNetCore.Authorization;
using SerializedStalker.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;
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
            var serie = await _serieRepository.GetAsync(input.SerieID);
            var userIdActual = _currentUserService.GetCurrentUserId();
            //Un usuario solo puede calificar las series relacionadas a él
            if (serie.CreatorId != userIdActual)
            {
                throw new UnauthorizedAccessException("No puedes calificar esta serie.");
            }

            //Un usuario no puede calificar 2 veces la misma serie
            var calificacionExistente = serie.Calificaciones.FirstOrDefault(c => c.UsuarioId == userIdActual);
            if (calificacionExistente != null)
            {
                throw new InvalidOperationException("Ya has calificado esta serie.");
            }

            var calificacion = new Calificacion
            {
                calificacion = input.calificacion,
                comentario = input.comentario,
                FechaCreacion = input.FechaCreacion,
                SerieID = input.SerieID,
                UsuarioId = userIdActual.Value // Asigna el ID del usuario actual
            };
            serie.Calificaciones.Add(calificacion);
            await _serieRepository.UpdateAsync(serie);
        }
    }
}
