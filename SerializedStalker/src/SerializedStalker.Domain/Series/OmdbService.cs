using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public class OmdbService : ISeriesApiService
    {
        public async Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero)
        {
            SerieDto[] series = new SerieDto[]
            {
                new SerieDto{ Titulo = "Game of Thrones"}
            };

            return await Task.FromResult(series);
        }
    }
}
