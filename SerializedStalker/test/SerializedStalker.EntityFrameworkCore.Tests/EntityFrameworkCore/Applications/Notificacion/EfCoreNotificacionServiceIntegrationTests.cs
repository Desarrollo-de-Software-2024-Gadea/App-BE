using SerializedStalker.Notificaciones;
using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace SerializedStalker.EntityFrameworkCore.Applications.Notificacion
{
    [Collection(SerializedStalkerTestConsts.CollectionDefinitionName)]
    public class EfCoreNotificacionServiceIntegrationTests : NotificacionServiceTests<SerializedStalkerEntityFrameworkCoreTestModule>
    {

    }
}
