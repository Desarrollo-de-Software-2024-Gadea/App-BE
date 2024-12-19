using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using Xunit;
using SerializedStalker.Series;
using Volo.Abp.Domain.Repositories;
using SerializedStalker.Notificaciones;
using System.Threading;
using System;
using Volo.Abp.ObjectMapping;

namespace SerializedStalker.Tests.Series
{
    public class SerieUpdateServiceTests
    {
        private readonly Mock<ISeriesApiService> _seriesApiServiceMock;
        private readonly Mock<IRepository<Serie, int>> _serieRepositoryMock;
        private readonly Mock<INotificacionService> _notificacionServiceMock;
        private readonly Mock<IObjectMapper> _objectMapper;
        private readonly SerieAppService _serieAppService;
        private readonly SerieUpdateService _serieUpdateService;

        public SerieUpdateServiceTests()
        {
            _seriesApiServiceMock = new Mock<ISeriesApiService>();
            _serieRepositoryMock = new Mock<IRepository<Serie, int>>();
            _notificacionServiceMock = new Mock<INotificacionService>();
            _objectMapper = new Mock<IObjectMapper>();

            _serieUpdateService = new SerieUpdateService(
                _seriesApiServiceMock.Object,
                _serieRepositoryMock.Object,
                _notificacionServiceMock.Object,
                _objectMapper.Object);
        }

        [Fact]
        public async Task Should_Update_Series_When_New_Season_Available()
        {
            // Arrange
            var serie = new Serie
            {
                Titulo = "Test Serie",
                TotalTemporadas = 1,
                Temporadas = new List<Temporada>()
            };

            var seriesList = new List<Serie> { serie };
            var apiSerie = new SerieDto { TotalTemporadas = 2, ImdbIdentificator = "tt123456" };

            _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(seriesList);

            _seriesApiServiceMock.Setup(s => s.BuscarSerieAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new[] { apiSerie });

            var nuevaTemporada = new TemporadaDto
            {
                NumeroTemporada = 2,
                Episodios = new List<EpisodioDto>
        {
            new EpisodioDto { NumeroEpisodio = 1, Titulo = "Nuevo episodio", FechaEstreno = DateOnly.FromDateTime(DateTime.Now) }
        }
            };

            _seriesApiServiceMock.Setup(s => s.BuscarTemporadaAsync(It.IsAny<string>(), 2))
                .ReturnsAsync(nuevaTemporada);

            // Act
            await _serieUpdateService.VerificarYActualizarSeriesAsync();

            // Assert
            _serieRepositoryMock.Verify(r => r.UpdateAsync(It.Is<Serie>(s => s.TotalTemporadas == 2), It.IsAny<bool>(), It.IsAny<CancellationToken>()), Times.Once);
            _notificacionServiceMock.Verify(n => n.CrearYEnviarNotificacionAsync(
                001,
                $"Nueva temporada disponible de {serie.Titulo}",
                $"La temporada 2 ya está disponible en {serie.Titulo}.",
                TipoNotificacion.Email), Times.Once);
        }


        [Fact]
        public async Task Should_Notify_When_New_Episodes_Available()
        {
            // Arrange
            var serie = new Serie
            {
                Titulo = "Test Serie",
                TotalTemporadas = 1,
                Temporadas = new List<Temporada>
        {
            new Temporada
            {
                NumeroTemporada = 1,
                Episodios = new List<Episodio>
                {
                    new Episodio { NumeroEpisodio = 1, Titulo = "Episodio 1", FechaEstreno = DateOnly.FromDateTime(DateTime.Now) }
                }
            }
        }
            };

            var seriesList = new List<Serie> { serie };
            var apiSerie = new SerieDto { TotalTemporadas = 1, ImdbIdentificator = "tt123456" };

            _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(seriesList);

            _seriesApiServiceMock.Setup(s => s.BuscarSerieAsync(It.IsAny<string>(), It.IsAny<string>()))
                .ReturnsAsync(new[] { apiSerie });

            var apiTemporada = new TemporadaDto
            {
                NumeroTemporada = 1,
                Episodios = new List<EpisodioDto>
        {
            new EpisodioDto { NumeroEpisodio = 1, Titulo = "Episodio 1", FechaEstreno = DateOnly.FromDateTime(DateTime.Now) },
            new EpisodioDto { NumeroEpisodio = 2, Titulo = "Nuevo Episodio 2", FechaEstreno = DateOnly.FromDateTime(DateTime.Now) }
        }
            };

            _seriesApiServiceMock.Setup(s => s.BuscarTemporadaAsync(It.IsAny<string>(), 1))
                .ReturnsAsync(apiTemporada);

            // Act
            await _serieUpdateService.VerificarYActualizarSeriesAsync();

            // Assert
            _notificacionServiceMock.Verify(n => n.CrearYEnviarNotificacionAsync(
                001,
                $"Nuevos episodios en {serie.Titulo}",
                "Se han añadido 1 nuevos episodios en la serie Test Serie.",
                TipoNotificacion.Email), Times.Once);
        }

       /* [Fact]
        public async Task Should_Persist_Series_If_Not_Exists()
        {
            // Arrange
            var serieDto = new SerieDto
            {
                ImdbIdentificator = "tt123456",
                Titulo = "Test Serie",
                TotalTemporadas = 1,
                Temporadas = new List<TemporadaDto>() // Asegúrate de inicializar la colección
            };

            var seriesList = new List<Serie>(); // La lista de series está vacía

            _serieRepositoryMock.Setup(r => r.GetListAsync(It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(seriesList); // Devolvemos una lista vacía

            // Act
            await _serieAppService.PersistirSeriesAsync(new[] { serieDto });

            // Assert
            // Verificar que se inserte la nueva serie
            _serieRepositoryMock.Verify(r => r.InsertAsync(
                It.Is<Serie>(s => s.ImdbIdentificator == serieDto.ImdbIdentificator && s.Titulo == serieDto.Titulo),
                false, // autoSave = false
                It.IsAny<CancellationToken>()), // CancellationToken
            Times.Once);
        }*/



    }
}
