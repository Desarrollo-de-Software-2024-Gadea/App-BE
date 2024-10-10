using System;
using System.Threading.Tasks;
using SerializedStalker.Application.Contracts.Notificaciones;
using SerializedStalker.Domain.Notificaciones;
using SerializedStalker.Notificaciones;

namespace SerializedStalker.Application.Notificaciones
{
    public class EmailNotificador : INotificador
    {
        public bool PuedeEnviar(TipoNotificacion tipo)
        {
            return tipo == TipoNotificacion.Email;
        }

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

            // Lógica para enviar un correo electrónico usando notificacion.Titulo y notificacion.Mensaje
            // Simulación de envío de correo:
            await Task.Delay(1000); // Simula el tiempo de envío de un correo
            Console.WriteLine($"Email enviado a usuario {notificacion.UsuarioId}: {notificacion.Titulo} - {notificacion.Mensaje}");
        }
    }
}
