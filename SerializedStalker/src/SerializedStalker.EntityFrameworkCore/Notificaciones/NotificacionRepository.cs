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
    public class NotificacionRepository : EfCoreRepository<SerializedStalkerDbContext, Notificacion, int>, INotificacionRepository
    {
        public NotificacionRepository(IDbContextProvider<SerializedStalkerDbContext> dbContextProvider)
            : base(dbContextProvider)
        {
        }

        public async Task<List<Notificacion>> GetNotificacionesNoLeidasAsync(int usuarioId)
        {
            return await DbSet
                .Where(n => n.UsuarioId == usuarioId && !n.Leida)
                .ToListAsync();
        }
    }
}
