using SerializedStalker.ListasDeSeguimiento;
using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Applications.Series
{
    [Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
    public class EfCoreMonitoreoApiAppServiceIntegrationTests : MonitoreoApiAppServiceTest<SerializedStalkerEntityFrameworkCoreTestModule>
    {

    }
}
