using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SerializedStalker.Application.Contracts.Notificaciones;
using SerializedStalker.Domain.Notificaciones;
using Volo.Abp.DependencyInjection;

namespace SerializedStalker.Notificaciones
{
    public class NotificacionService : INotificacionService, ITransientDependency
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IEnumerable<INotificador> _notificadores;
        private readonly ILogger<NotificacionService> _logger;

        /// <summary>
        /// Inicializa una nueva instancia de la clase <see cref="NotificacionService"/>.
        /// </summary>
        /// <param name="notificacionRepository">El repositorio de notificaciones.</param>
        /// <param name="notificadores">La colección de notificadores disponibles.</param>
        /// <param name="logger">El logger para registrar información y errores.</param>
        public NotificacionService(
            INotificacionRepository notificacionRepository,
            IEnumerable<INotificador> notificadores,
            ILogger<NotificacionService> logger)
        {
            _notificacionRepository = notificacionRepository;
            _notificadores = notificadores;
            _logger = logger;
        }

        /// <summary>
        /// Crea una nueva notificación y la envía utilizando los notificadores registrados.
        /// </summary>
        /// <param name="usuarioId">El identificador del usuario al que se enviará la notificación.</param>
        /// <param name="titulo">El título de la notificación.</param>
        /// <param name="mensaje">El mensaje de la notificación.</param>
        /// <param name="tipo">El tipo de notificación.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="Exception">Se lanza si hay un error al insertar la notificación en el repositorio o al enviar la notificación.</exception>
        public async Task CrearYEnviarNotificacionAsync(int usuarioId, string titulo, string mensaje, TipoNotificacion tipo)
        {
            _logger.LogInformation("Creando notificación para el usuario {UsuarioId} con título {Titulo}", usuarioId, titulo);

            // Crear un DTO para la notificación
            var notificacionDto = new NotificacionDto
            {
                UsuarioId = usuarioId,
                Titulo = titulo,
                Mensaje = mensaje,
                Leida = false,
                Tipo = tipo
            };

            // Insertar la notificación en el repositorio
            var notificacion = new Notificacion
            {
                UsuarioId = usuarioId,
                Titulo = titulo,
                Mensaje = mensaje,
                Leida = false,
                Tipo = tipo
            };

            try
            {
                await _notificacionRepository.InsertAsync(notificacion);
                _logger.LogInformation("Notificación insertada en el repositorio para el usuario {UsuarioId}", usuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al insertar la notificación en el repositorio para el usuario {UsuarioId}", usuarioId);
                throw;
            }

            // Enviar la notificación utilizando los notificador(es) registrados
            var notificadoresFiltrados = _notificadores.Where(n => n.PuedeEnviar(tipo));
            foreach (var notificador in notificadoresFiltrados)
            {
                try
                {
                    await notificador.EnviarNotificacionAsync(notificacionDto); // Usar el DTO aquí
                    _logger.LogInformation("Notificación enviada utilizando {Notificador}", notificador.GetType().Name);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al enviar la notificación utilizando {Notificador}", notificador.GetType().Name);
                }
            }
        }
    }
}