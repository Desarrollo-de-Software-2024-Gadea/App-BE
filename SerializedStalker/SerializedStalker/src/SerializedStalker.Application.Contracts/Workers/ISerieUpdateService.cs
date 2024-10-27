using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public interface ISerieUpdateService
    {
        Task VerificarYActualizarSeriesAsync();
    }
}
