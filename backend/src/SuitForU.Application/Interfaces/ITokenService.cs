using System.Security.Claims;

namespace SuitForU.Application.Interfaces;

public interface ITokenService
{
    string GenerateAccessToken(Guid userId, string email);
    string GenerateRefreshToken(Guid userId);
    Guid? ValidateAccessToken(string token);
    ClaimsPrincipal? ValidateRefreshToken(string token);
}
