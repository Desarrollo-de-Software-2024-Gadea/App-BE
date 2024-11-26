using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;

namespace SerializedStalker.ListasDeSeguimiento
{
    public class ListaDeSeguimientoAppService : ApplicationService, IListaDeSeguimientoAppService
    {
        private readonly IRepository<ListaDeSeguimiento,int> _listaDeSeguimientoRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        public ListaDeSeguimientoAppService(IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository, IRepository<Serie, int> serieRepository)
        { 
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
            _serieRepository = serieRepository; 
        }
        public async Task AddSerieAsync(int SerieID)
        {
            var listaDeSeguimiento = ((List<ListaDeSeguimiento>) await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault();

            if (listaDeSeguimiento == null) 
            { 
                listaDeSeguimiento = new ListaDeSeguimiento();
                await _listaDeSeguimientoRepository.InsertAsync(listaDeSeguimiento); 
            }

            var Serie = await _serieRepository.GetAsync(SerieID);
            listaDeSeguimiento.Series.Add(Serie);
            await _listaDeSeguimientoRepository.UpdateAsync(listaDeSeguimiento);

        }
    }
}
