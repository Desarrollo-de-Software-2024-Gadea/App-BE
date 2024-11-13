using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using SerializedStalker.Series;

namespace SerializedStalker.ListasDeSeguimiento
{
    public interface IListaDeSeguimientoAppService : IApplicationService
    {
        Task AddSerieAsync(SerieDto serieDto);
        Task<SerieDto[]> MostrarSeriesAsync();
        Task EliminarSerieAsync(string ImdbID);
    }
}
