using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public interface ISeriesApiService
    {
        Task<SerieDto[]> BuscarSerieAsync(string titulo, string genero);
    }
}
