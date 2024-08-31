using Xunit;

namespace SerializedStalker.EntityFrameworkCore;

[CollectionDefinition(SerializedStalkerTestConsts.CollectionDefinitionName)]
public class SerializedStalkerEntityFrameworkCoreCollection : ICollectionFixture<SerializedStalkerEntityFrameworkCoreFixture>
{

}
