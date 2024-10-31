using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using Microsoft.AspNetCore.Authorization;


namespace SerializedStalker.ListasDeSeguimiento
{
    [Authorize]
    public class ListaDeSeguimientoAppService : ApplicationService, IListaDeSeguimientoAppService
    {
        private readonly IRepository<ListaDeSeguimiento,int> _listaDeSeguimientoRepository;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly ICurrentUser _currentUser;
        private readonly OmdbService _service;
        private readonly SerieUpdateService _serieUpService;

        public ListaDeSeguimientoAppService(IRepository<ListaDeSeguimiento, int> listaDeSeguimientoRepository, IRepository<Serie, int> serieRepository, ICurrentUser currentUser)
        { 
            _listaDeSeguimientoRepository = listaDeSeguimientoRepository;
            _serieRepository = serieRepository;
            _currentUser = currentUser;
        }
        public async Task AddSerieAsync(string titulo) //int serieID
        {
            //var userEnt = _currentUser;
            Guid userId = (Guid)_currentUser.Id;
            // Obtén la lista de seguimiento, asumiendo que solo hay una por ahora
            var listaDeSeguimiento = (await _listaDeSeguimientoRepository.GetListAsync()).FirstOrDefault();

            // Si no existe, crea una nueva lista de seguimiento
            if (listaDeSeguimiento == null)
            {
                listaDeSeguimiento = new ListaDeSeguimiento();
                await _listaDeSeguimientoRepository.InsertAsync(listaDeSeguimiento);
            }

            //Obtenemos el serieDto
            var serieApi = await _service.BuscarSerieAsync(titulo, null);
            // Busca la serie por ID
            //var serie = await _serieRepository.GetAsync(serieID);

            // Comprueba si la serie ya está en la lista
            if (!listaDeSeguimiento.Series.Any(s => s.ImdbIdentificator == serieApi.FirstOrDefault().ImdbIdentificator))
            {
                await _serieUpService.PersistirSeriesAsync(serieApi, userId);
                var serie = (await _serieRepository.GetListAsync()).LastOrDefault();
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

/*
Como la idea del profe es que las series se persistan por separado, 
debemos buscar la serie en la api y ahí persistirla, para luego agregarla a la lista de seguimiento.
Debemos tambien tener en cuenta el usuario que la agrega a su lista así lo ponemos en la serie.
*/