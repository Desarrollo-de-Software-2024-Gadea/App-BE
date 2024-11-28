using SerializedStalker.Notificaciones;
using System.Threading.Tasks;

namespace SerializedStalker.Application.Contracts.Notificaciones
{
    public interface INotificador
    {
        bool PuedeEnviar(TipoNotificacion tipo);
        Task EnviarNotificacionAsync(NotificacionDto notificacion);
    }
}
