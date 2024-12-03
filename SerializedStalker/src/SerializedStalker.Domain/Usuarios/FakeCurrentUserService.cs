using SerializedStalker.Usuarios;
using System;

namespace SerializedStalker.EntityFrameworkCore
{
    public class FakeCurrentUserService : ICurrentUserService
    {
        public Guid? GetCurrentUserId()
        {
            return Guid.NewGuid();
        }
    }
}