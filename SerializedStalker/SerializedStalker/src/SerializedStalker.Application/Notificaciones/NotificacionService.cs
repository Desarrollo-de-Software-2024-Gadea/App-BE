using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SerializedStalker.Application.Contracts.Notificaciones;
using SerializedStalker.Domain.Notificaciones;
using Volo.Abp.DependencyInjection;

namespace SerializedStalker.Notificaciones
{
    public class NotificacionService : INotificacionService, ITransientDependency
    {
        private readonly INotificacionRepository _notificacionRepository;
        private readonly IEnumerable<INotificador> _notificadores;

        public NotificacionService(
            INotificacionRepository notificacionRepository,
            IEnumerable<INotificador> notificadores)
        {
            _notificacionRepository = notificacionRepository;
            _notificadores = notificadores;
        }

        public async Task CrearYEnviarNotificacionAsync(int usuarioId, string titulo, string mensaje, TipoNotificacion tipo)
        {
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

            await _notificacionRepository.InsertAsync(notificacion);

            // Enviar la notificación utilizando los notificador(es) registrados
            var notificadoresFiltrados = _notificadores.Where(n => n.PuedeEnviar(tipo));
            foreach (var notificador in notificadoresFiltrados)
            {
                await notificador.EnviarNotificacionAsync(notificacionDto); // Usar el DTO aquí
            }
        }
    }
}
