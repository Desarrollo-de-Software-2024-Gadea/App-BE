using SerializedStalker.Localization;
using Volo.Abp.Authorization.Permissions;
using Volo.Abp.Localization;
using Volo.Abp.MultiTenancy;

namespace SerializedStalker.Permissions;

public class SerializedStalkerPermissionDefinitionProvider : PermissionDefinitionProvider
{
    public override void Define(IPermissionDefinitionContext context)
    {
        var myGroup = context.AddGroup(SerializedStalkerPermissions.GroupName);

        //Define your own permissions here. Example:
        //myGroup.AddPermission(SerializedStalkerPermissions.MyPermission1, L("Permission:MyPermission1"));
    }

    private static LocalizableString L(string name)
    {
        return LocalizableString.Create<SerializedStalkerResource>(name);
    }
}