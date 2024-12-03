using SerializedStalker.Samples;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Domains;

[Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
public class EfCoreSampleDomainTests : SampleDomainTests<SerializedStalkerEntityFrameworkCoreTestModule>
{

}
