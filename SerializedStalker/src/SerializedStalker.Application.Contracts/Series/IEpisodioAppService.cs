using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Application.Services;

namespace SerializedStalker.Series
{
    public interface IEpisodioAppService : ICrudAppService<EpisodioDto, int, PagedAndSortedResultRequestDto, CreateUpdateEpisodioDto, CreateUpdateEpisodioDto>
    {
    }
}
