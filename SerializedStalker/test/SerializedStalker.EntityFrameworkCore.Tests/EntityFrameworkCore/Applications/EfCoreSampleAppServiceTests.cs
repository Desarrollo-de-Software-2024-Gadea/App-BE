using SerializedStalker.Samples;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Applications;

[Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleAppServiceTests : SampleAppServiceTests<SerializedStalkerEntityFrameworkCoreTestModule>
{

}
