using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Domains.Series
{

    [Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
    public class EfCoreOmdbService_Tests : OmdbService_Tests<SerializedStalkerEntityFrameworkCoreTestModule>
    {

    }
}
