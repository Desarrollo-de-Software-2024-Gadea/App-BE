using System;

namespace SerializedStalker.Usuarios
{
    public interface ICurrentUserService
    {
        Guid? GetCurrentUserId();
    }
}