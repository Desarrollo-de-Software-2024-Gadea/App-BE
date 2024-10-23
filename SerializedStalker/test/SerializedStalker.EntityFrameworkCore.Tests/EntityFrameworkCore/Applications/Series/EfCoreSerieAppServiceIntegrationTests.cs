using SerializedStalker.Samples;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Applications.Series
{
    
    [Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
    public class EfCoreSerieAppServiceIntegrationTests : SerieAppServiceIntegrationTests<SerializedStalkerEntityFrameworkCoreTestModule>
    {

    }
}
