using System;
using System.Threading.Tasks;

namespace SerializedStalker.Notificaciones
{
    public interface INotificacionService
    {
        Task CrearYEnviarNotificacionAsync(int usuarioId, string titulo, string mensaje, TipoNotificacion tipo);
    }
}
