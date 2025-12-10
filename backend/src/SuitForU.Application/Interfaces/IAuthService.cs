using SuitForU.Application.DTOs.Auth;

namespace SuitForU.Application.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> ExternalAuthAsync(ExternalAuthDto externalAuthDto, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default);
    Task<bool> ConfirmEmailAsync(Guid userId, string token, CancellationToken cancellationToken = default);
    Task<UserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default);
}
