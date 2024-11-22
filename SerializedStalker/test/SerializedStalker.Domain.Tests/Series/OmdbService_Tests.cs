using System;
using System.Linq;
using System.Threading.Tasks;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Volo.Abp.Validation;
using Xunit;

namespace SerializedStalker.Series
{
    public abstract class OmdbService_Tests<TStartupModule> : SerializedStalkerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly OmdbService _service;

        protected OmdbService_Tests()
        {
            _service = GetRequiredService<OmdbService>();
        }

        [Fact]
        public async Task Buscar_Serie_Ejemplo()
        {
            //arrange
            var titulo = "Mobile Suit Gundam";
            //Act
            var result = await _service.BuscarSerieAsync(titulo, string.Empty);

            //Assert
            //result.Count.ShouldBeGreaterThan(0);
            result.ShouldNotBeNull();
            result.ShouldContain(b => b.Titulo == titulo);
        }
        [Fact]
        public async Task Serie_No_Existente()
        {
            //arrange
            var titulo = "IGJEMGIDSAJ8faunefag2681";
            //Act
            var result = await _service.BuscarSerieAsync(titulo, string.Empty);

            //Assert
            result.ShouldNotBeEmpty();
        }
    }
}
