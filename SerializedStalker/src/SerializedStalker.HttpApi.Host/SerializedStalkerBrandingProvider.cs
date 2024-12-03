using Microsoft.Extensions.Localization;
using SerializedStalker.Localization;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Ui.Branding;

namespace SerializedStalker;

[Dependency(ReplaceServices = true)]
public class SerializedStalkerBrandingProvider : DefaultBrandingProvider
{
    private IStringLocalizer<SerializedStalkerResource> _localizer;

    public SerializedStalkerBrandingProvider(IStringLocalizer<SerializedStalkerResource> localizer)
    {
        _localizer = localizer;
    }

    public override string AppName => _localizer["AppName"];
}
