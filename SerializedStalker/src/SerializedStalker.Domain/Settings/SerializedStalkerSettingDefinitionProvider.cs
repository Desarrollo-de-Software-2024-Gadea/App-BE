using Volo.Abp.Settings;

namespace SerializedStalker.Settings;

public class SerializedStalkerSettingDefinitionProvider : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        //Define your own settings here. Example:
        //context.Add(new SettingDefinition(SerializedStalkerSettings.MySetting1));
    }
}
