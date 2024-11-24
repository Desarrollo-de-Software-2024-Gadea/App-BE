using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SerializedStalker.Series
{
    public interface ISerieAppService : ICrudAppService<SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>
    {
        Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero = null); // Hacer que el genero sea opcional
        Task<TemporadaDto> BuscarTemporadaAsync(string imdbId, int numeroTemporada);
        Task CalificarSerieAsync(CalificacionDto input);
        Task PersistirSeriesAsync(SerieDto[] seriesDto);
        Task<SerieDto[]> ObtenerSeriesAsync();
    }
}

