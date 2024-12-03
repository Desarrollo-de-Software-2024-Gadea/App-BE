using SerializedStalker.Localization;
using Volo.Abp.AspNetCore.Mvc;

namespace SerializedStalker.Controllers;

/* Inherit your controllers from this class.
 */
public abstract class SerializedStalkerController : AbpControllerBase
{
    protected SerializedStalkerController()
    {
        LocalizationResource = typeof(SerializedStalkerResource);
    }
}
