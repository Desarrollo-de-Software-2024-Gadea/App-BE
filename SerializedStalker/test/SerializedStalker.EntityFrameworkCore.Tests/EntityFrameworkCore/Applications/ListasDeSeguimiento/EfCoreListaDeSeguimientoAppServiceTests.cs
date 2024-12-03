using SerializedStalker.ListasDeSeguimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Applications.ListasDeSeguimiento
{
    [Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
    public class EfCoreListaDeSeguimientoAppServiceIntegrationTests : ListaDeSeguimientoAppServiceTests<SerializedStalkerEntityFrameworkCoreTestModule>
    {

    }

}
