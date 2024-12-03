using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;

namespace SerializedStalker.ListasDeSeguimiento
{
    public interface IListaDeSeguimientoAppService : IApplicationService
    {
        Task AddSerieAsync(int SerieID); 
    }
}
