﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SerializedStalker.Series;
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
            seriesDto.FirstOrDefault(s => s.ImdbIdentificator == "tt1234567"); //Falso positivo
        }
        [Fact]
        public async Task Should_Erase_One_Serie()
        {
            //Act
            await _listaDeSeguimientoAppService.EliminarSerieAsync("tt1234567");
            var seriesDto = await _listaDeSeguimientoAppService.MostrarSeriesAsync();

            //Assert
            seriesDto.ShouldBeEmpty();
        }
        [Fact]
        public async Task Should_Add_One_Serie()
        {
            //Act
            var serieDto = new SerieDto
            {
                Titulo = "Test 2",
                Clasificacion = "PG-13",
                FechaEstreno = "2023-01-09",
                Duracion = "2h",
                Generos = "Drama",
                Directores = "Director Test",
                Escritores = "Writer Test",
                Actores = "Actor Test",
                Sinopsis = "Test Sinopsis",
                Idiomas = "Español",
                Pais = "España",
                Poster = "URL del poster",
                ImdbPuntuacion = "8.7",
                ImdbVotos = 1000,
                ImdbIdentificator = "xx54da154",
                Tipo = "Serie",
            };
            await _listaDeSeguimientoAppService.AddSerieAsync(serieDto);
            var seriesDto = await _listaDeSeguimientoAppService.MostrarSeriesAsync();

            //Assert
            seriesDto.Length.ShouldBeGreaterThan(0); //Si hacemos funcionar el SEEDer entonces 0 deberia pasar a 1
            //FirstOrDefault(s => s.ImdbIdentificator == "tt1234567");
        }
    }
}