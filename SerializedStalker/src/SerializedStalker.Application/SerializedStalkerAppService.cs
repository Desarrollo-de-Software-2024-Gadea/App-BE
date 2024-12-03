using SerializedStalker.Localization;
using Volo.Abp.Application.Services;

namespace SerializedStalker;

/* Inherit your application services from this class.
 */
public abstract class SerializedStalkerAppService : ApplicationService
{
    protected SerializedStalkerAppService()
    {
        LocalizationResource = typeof(SerializedStalkerResource);
    }
}
