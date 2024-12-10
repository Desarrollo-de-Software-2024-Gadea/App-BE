using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SerializedStalker.EntityFrameworkCore;
using SerializedStalker.Notificaciones;
using Shouldly;
using Volo.Abp.Application.Dtos;
using Volo.Abp.Modularity;
using Xunit;

namespace SerializedStalker.Notificaciones
{
    public abstract class NotificacionServiceTests<TStartupModule> : SerializedStalkerTestBase<TStartupModule>
    where TStartupModule : IAbpModule
    {
        private readonly INotificacionService _notificacionService;
        private readonly SerializedStalkerDbContext _dbContext;

        protected NotificacionServiceTests()
        {
            _notificacionService = GetRequiredService<INotificacionService>();
            _dbContext = GetRequiredService<SerializedStalkerDbContext>();
        }

        /// <summary>
        /// Verifica que el método <c>MostrarNotificacionesPantalla</c> retorne una lista de notificaciones no vacía
        /// cuando se llama con un usuario que tiene notificaciones no leídas.
        /// </summary>
        [Fact]
        public void MostrarNotificacionesPantalla_Should_Return_Unread_Notifications()
        {
            // Arrange
            var usuarioId = 1;

            // Act
            var notificacionesDto = _notificacionService.MostrarNotificacionesPantalla(usuarioId);

            // Assert
            notificacionesDto.ShouldNotBeEmpty();
            notificacionesDto.First().UsuarioId.ShouldBe(usuarioId);
        }

        /// <summary>
        /// Verifica que el método <c>CrearYEnviarNotificacionAsync</c> cree y envíe una notificación correctamente.
        /// </summary>
        [Fact]
        public async Task CrearYEnviarNotificacionAsync_Should_Create_And_Send_Notification()
        {
            // Arrange
            var usuarioId = 1;
            var titulo = "Nuevo Titulo";
            var mensaje = "Nuevo Mensaje";
            var tipo = TipoNotificacion.Email;

            // Act
            await _notificacionService.CrearYEnviarNotificacionAsync(usuarioId, titulo, mensaje, tipo);

            // Assert: verifica en la base de datos
            var notificacionEnDb = await _dbContext.Notificaciones
                .FirstOrDefaultAsync(n => n.UsuarioId == usuarioId && n.Titulo == titulo && n.Mensaje == mensaje);

            notificacionEnDb.ShouldNotBeNull(); // Verifica que la notificación fue guardada
            notificacionEnDb.Titulo.ShouldBe(titulo); // Verifica que los datos coinciden
        }
    }
}
