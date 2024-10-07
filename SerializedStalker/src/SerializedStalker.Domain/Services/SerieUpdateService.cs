using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using SerializedStalker.Series;

namespace SerializedStalker.Series
{
    public class SerieUpdateService : DomainService
    {
        private readonly ISeriesApiService _seriesApiService;
        private readonly IRepository<Serie, int> _serieRepository;

        public SerieUpdateService(
            ISeriesApiService seriesApiService,
            IRepository<Serie, int> serieRepository)
        {
            _seriesApiService = seriesApiService;
            _serieRepository = serieRepository;
        }

        public async Task VerificarYActualizarSeriesAsync()
        {
            var series = await _serieRepository.GetListAsync();

            foreach (var serie in series)
            {
                var apiSeries = await _seriesApiService.BuscarSerieAsync(serie.Titulo, null);

                if (apiSeries != null && apiSeries.Length > 0)
                {
                    var apiSerie = apiSeries.FirstOrDefault();

                    if (apiSerie.TotalTemporadas > serie.TotalTemporadas)
                    {
                        serie.TotalTemporadas = apiSerie.TotalTemporadas;
                        await _serieRepository.UpdateAsync(serie);
                    }
                }
            }
        }
    }
}
