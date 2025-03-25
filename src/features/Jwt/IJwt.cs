using System.Security.Claims;
using UserModel = FnbReservationAPI.src.features.User.User;

namespace FnbReservationAPI.src.features.Jwt
{
    public interface IJwtService
    {
        string GenerateToken(UserModel user);
        ClaimsPrincipal? ValidateToken(string token);
        bool IsTokenActive(string userId, string token);
        void InvalidateToken(string userId);
    }
}