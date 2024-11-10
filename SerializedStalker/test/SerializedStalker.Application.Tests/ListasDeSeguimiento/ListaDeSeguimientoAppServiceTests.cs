using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace SerializedStalker.ListasDeSeguimiento
{
    public abstract class ListaDeSeguimientoAppServiceTests<TStartupModule> : SerializedStalkerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly IListaDeSeguimientoAppService _listaDeSeguimientoAppService;

        protected ListaDeSeguimientoAppServiceTests()
        {
            _listaDeSeguimientoAppService = GetRequiredService<IListaDeSeguimientoAppService>();
        }
        [Fact]
        public async Task Should_Show_Series_Of_The_List()
        {
            //Act
            var seriesDto = await _listaDeSeguimientoAppService.MostrarSeriesAsync();

            //Assert
            seriesDto.FirstOrDefault(s => s.ImdbIdentificator == "tt1234567");
        }
        [Fact]
        public async Task Should_Erase_One_Serie()
        {
            //Act
            await _listaDeSeguimientoAppService.EliminarSerieAsync("tt1234567");
            var seriesDto = await _listaDeSeguimientoAppService.MostrarSeriesAsync();

            //Assert
            seriesDto.ShouldBeEmpty();
                //FirstOrDefault(s => s.ImdbIdentificator == "tt1234567");
        }
        [Fact]
        public async Task Should_Erase_Add_Serie()
        {
            //Act
            await _listaDeSeguimientoAppService.AddSerieAsync("Gundam");
            var seriesDto = await _listaDeSeguimientoAppService.MostrarSeriesAsync();

            //Assert
            seriesDto.Length.ShouldBeGreaterThan(2);
            //FirstOrDefault(s => s.ImdbIdentificator == "tt1234567");
        }
    }
}
