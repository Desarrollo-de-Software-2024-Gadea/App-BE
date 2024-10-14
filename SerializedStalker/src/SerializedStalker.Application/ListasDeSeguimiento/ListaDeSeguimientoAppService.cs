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
        public async Task AddSerieAsync(int serieID)
        {
            // Obtén la lista de seguimiento, asumiendo que solo hay una por ahora
            var listaDeSeguimiento = (await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault();

            // Si no existe, crea una nueva lista de seguimiento
            if (listaDeSeguimiento == null)
            {
                listaDeSeguimiento = new ListaDeSeguimiento();
                await _listaDeSeguimientoRepository.InsertAsync(listaDeSeguimiento);
            }

            // Busca la serie por ID
            var serie = await _serieRepository.GetAsync(serieID);

            // Comprueba si la serie ya está en la lista
            if (!listaDeSeguimiento.Series.Any(s => s.ImdbIdentificator == serie.ImdbIdentificator))
            {
                listaDeSeguimiento.Series.Add(serie); // Añade la serie a la lista
            }
            else
            {
                throw new Exception("La serie ya está en la lista de seguimiento."); // Maneja el caso en que la serie ya está en la lista
            }

            // Actualiza la lista de seguimiento en la base de datos
            await _listaDeSeguimientoRepository.UpdateAsync(listaDeSeguimiento);
        }

    }
}
