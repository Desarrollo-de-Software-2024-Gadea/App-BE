using SerializedStalker.Application.Contracts.Notificaciones;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SerializedStalker.Notificaciones
{
    public interface INotificacionService
    {
        List<NotificacionDto> MostrarNotificacionesPantalla(int usuarioId);
        Task CrearYEnviarNotificacionAsync(int usuarioId, string titulo, string mensaje, TipoNotificacion tipo);
    }
}
