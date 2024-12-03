using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SerializedStalker.Application.Contracts.Notificaciones;
using SerializedStalker.Domain.Notificaciones;
using SerializedStalker.Notificaciones;

namespace SerializedStalker.Application.Notificaciones
{
    /// <summary>
    /// Inicializa una nueva instancia de la clase <see cref="EmailNotificador"/>.
    /// </summary>
    /// <param name="logger">El logger para registrar información y errores.</param>
    public class EmailNotificador : INotificador
    {
        private readonly ILogger<EmailNotificador> _logger;

        public EmailNotificador(ILogger<EmailNotificador> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Determina si este notificador puede enviar notificaciones del tipo especificado.
        /// </summary>
        /// <param name="tipo">El tipo de notificación.</param>
        /// <returns><c>true</c> si este notificador puede enviar notificaciones del tipo especificado; de lo contrario, <c>false</c>.</returns>
        public bool PuedeEnviar(TipoNotificacion tipo)
        {
            return tipo == TipoNotificacion.Email;
        }

        /// <summary>
        /// Envía una notificación por correo electrónico.
        /// </summary>
        /// <param name="notificacionDto">El DTO de la notificación a enviar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        /// <exception cref="Exception">Se lanza si hay un error al enviar el correo electrónico.</exception>
        public async Task EnviarNotificacionAsync(NotificacionDto notificacionDto)
        {
            // Convertir DTO a entidad de dominio
            var notificacion = new Notificacion
            {
                UsuarioId = notificacionDto.UsuarioId,
                Titulo = notificacionDto.Titulo,
                Mensaje = notificacionDto.Mensaje,
                Leida = false,
                Tipo = notificacionDto.Tipo,
            };

            try
            {
                _logger.LogInformation("Enviando notificación por correo electrónico al usuario {UsuarioId} con título {Titulo}", notificacion.UsuarioId, notificacion.Titulo);

                // Lógica para enviar un correo electrónico usando notificacion.Titulo y notificacion.Mensaje
                // Simulación de envío de correo:
                await Task.Delay(1000); // Simula el tiempo de envío de un correo
                Console.WriteLine($"Email enviado a usuario {notificacion.UsuarioId}: {notificacion.Titulo} - {notificacion.Mensaje}");

                _logger.LogInformation("Correo electrónico enviado exitosamente al usuario {UsuarioId}", notificacion.UsuarioId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al enviar el correo electrónico al usuario {UsuarioId}", notificacion.UsuarioId);
                throw;
            }
        }
    }
}

