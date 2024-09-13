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
    internal class EpisodioAppService : CrudAppService<Episodio, EpisodioDto, int, PagedAndSortedResultRequestDto, CreateUpdateEpisodioDto, CreateUpdateEpisodioDto>, IEpisodioAppService
    {
        public EpisodioAppService(IRepository<Episodio, int> repository) : base(repository)
        {
        }
    }
}
