using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SerializedStalker.Domain.Notificaciones;
using Volo.Abp.Domain.Repositories.EntityFrameworkCore;
using Volo.Abp.EntityFrameworkCore;

namespace SerializedStalker.EntityFrameworkCore.Notificaciones
{
    /*public class NotificacionRepository : EfCoreRepository<MyDbContext, Notificacion, int>, INotificacionRepository
    {
        public NotificacionRepository(IDbContextProvider<MyDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Notificacion>> GetNotificacionesNoLeidasAsync(Guid usuarioId)
        {
            return await DbSet
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .ToListAsync();
        }
    }*/
}
