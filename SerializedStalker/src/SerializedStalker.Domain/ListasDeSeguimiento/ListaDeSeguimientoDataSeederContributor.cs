using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Data;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace SerializedStalker.ListasDeSeguimiento
{
    public class ListaDeSeguimientoDataSeederContributor : IDataSeedContributor, ITransientDependency
    {
        private readonly IRepository<ListaDeSeguimiento, int> _listaDeSeguimientoRepository;

        public ListaDeSeguimientoDataSeederContributor(IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository)
        {
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
        }

        public async Task SeedAsync(DataSeedContext context)
        {
            if (await _listaDeSeguimientoRepository.GetCountAsync() <= 0)
            {
                var nuevaLista = new ListaDeSeguimiento
                {
                    FechaModificacion = DateOnly.FromDateTime(DateTime.Now),
                };
                var serieExistente = new Serie
                {
                    ImdbIdentificator = "ID_Falso01",

                };
/*                await _listaDeSeguimientoRepository.InsertAsync(

                    autoSave: true
                );
*/
            }
        }
    }
}
