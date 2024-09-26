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
    public class SerieAppService : CrudAppService<Serie, SerieDto, int, PagedAndSortedResultRequestDto, CreateUpdateSerieDto, CreateUpdateSerieDto>, ISerieAppService
    {
        private readonly ISeriesApiService _seriesApiService;
        public SerieAppService(IRepository<Serie, int> repository, ISeriesApiService seriesApiService) : base(repository)
        {
            _seriesApiService = seriesApiService;
        }

        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero)
        {
            return await _seriesApiService.BuscarSerieAsync(titulo, genero);
        }
    }
}
