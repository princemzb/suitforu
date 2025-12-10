using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using AutoMapper;
using Microsoft.Extensions.Logging;
using SuitForU.Application.DTOs.Auth;
using SuitForU.Application.Interfaces;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Enums;
using SuitForU.Domain.Interfaces;

namespace SuitForU.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUnitOfWork unitOfWork,
        ITokenService tokenService,
        IMapper mapper,
        ILogger<AuthService> logger)
    {
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Vérifier si l'email existe déjà
            var existingUser = await _unitOfWork.Users.GetByEmailAsync(registerDto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Un utilisateur avec cet email existe déjà.");
            }

            // Créer le nouvel utilisateur
            var user = new User
            {
                Email = registerDto.Email,
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                PhoneNumber = registerDto.PhoneNumber,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                AuthProvider = AuthProvider.Local,
                EmailConfirmed = false,
                PhoneNumberConfirmed = false
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Nouvel utilisateur créé : {Email}", user.Email);

            // Générer les tokens
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
            var refreshTokenString = _tokenService.GenerateRefreshToken(user.Id);

            // Stocker le refresh token en base de données
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress ?? "Unknown"
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'inscription de l'utilisateur {Email}", registerDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Rechercher l'utilisateur
            var user = await _unitOfWork.Users.GetByEmailAsync(loginDto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");
            }

            // Vérifier que c'est un compte local
            if (user.AuthProvider != AuthProvider.Local)
            {
                throw new InvalidOperationException($"Ce compte utilise l'authentification {user.AuthProvider}. Veuillez vous connecter avec {user.AuthProvider}.");
            }

            // Vérifier le mot de passe
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Email ou mot de passe incorrect.");
            }

            // Mettre à jour la date de dernière connexion
            user.LastLoginAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Utilisateur connecté : {Email}", user.Email);

            // Générer les tokens
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
            var refreshTokenString = _tokenService.GenerateRefreshToken(user.Id);

            // Stocker le refresh token en base de données
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress ?? "Unknown"
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la connexion de l'utilisateur {Email}", loginDto.Email);
            throw;
        }
    }

    public async Task<AuthResponseDto> RefreshTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Récupérer le refresh token depuis la base de données
            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
            
            if (storedRefreshToken == null)
            {
                throw new UnauthorizedAccessException("Refresh token invalide.");
            }

            // Vérifier si le token est actif (non révoqué et non expiré)
            if (!storedRefreshToken.IsActive)
            {
                // Si le token a été remplacé, révoquer toute la chaîne (détection de réutilisation)
                if (!string.IsNullOrEmpty(storedRefreshToken.ReplacedByToken))
                {
                    _logger.LogWarning("Tentative de réutilisation d'un refresh token révoqué pour l'utilisateur {UserId}", storedRefreshToken.UserId);
                    await _unitOfWork.RefreshTokens.RevokeAllUserTokensAsync(storedRefreshToken.UserId, cancellationToken);
                }
                
                throw new UnauthorizedAccessException("Refresh token invalide ou expiré.");
            }

            // Récupérer l'utilisateur
            var user = await _unitOfWork.Users.GetByIdAsync(storedRefreshToken.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Utilisateur introuvable.");
            }

            // Générer de nouveaux tokens (rotation)
            var newAccessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
            var newRefreshTokenString = _tokenService.GenerateRefreshToken(user.Id);

            // Créer le nouveau refresh token en base
            var newRefreshToken = new RefreshToken
            {
                Token = newRefreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress ?? "Unknown"
            };

            await _unitOfWork.RefreshTokens.AddAsync(newRefreshToken);

            // Révoquer l'ancien refresh token avec rotation
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            storedRefreshToken.RevokedByIp = ipAddress;
            storedRefreshToken.ReplacedByToken = newRefreshTokenString;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Refresh token utilisé avec succès pour l'utilisateur {UserId}", user.Id);

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenString,
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors du rafraîchissement du token");
            throw;
        }
    }

    public async Task<AuthResponseDto> ExternalAuthAsync(ExternalAuthDto externalAuthDto, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            // Déterminer le provider
            AuthProvider provider = externalAuthDto.Provider.ToLower() switch
            {
                "google" => AuthProvider.Google,
                "facebook" => AuthProvider.Facebook,
                "instagram" => AuthProvider.Instagram,
                _ => throw new ArgumentException($"Provider non supporté : {externalAuthDto.Provider}")
            };

            // Rechercher l'utilisateur par provider et ID externe
            var user = await _unitOfWork.Users.GetByExternalProviderAsync(provider, externalAuthDto.ExternalId);

            if (user == null)
            {
                // Créer un nouveau compte
                user = new User
                {
                    Email = externalAuthDto.Email,
                    FirstName = externalAuthDto.FirstName ?? string.Empty,
                    LastName = externalAuthDto.LastName ?? string.Empty,
                    ProfilePictureUrl = externalAuthDto.ProfilePictureUrl,
                    AuthProvider = provider,
                    ExternalProviderId = externalAuthDto.ExternalId,
                    EmailConfirmed = true, // Les emails OAuth sont déjà vérifiés
                    PasswordHash = string.Empty // Pas de mot de passe pour OAuth
                };

                await _unitOfWork.Users.AddAsync(user);
                _logger.LogInformation("Nouvel utilisateur OAuth créé : {Email} via {Provider}", user.Email, provider);
            }
            else
            {
                // Mettre à jour les informations si nécessaire
                user.LastLoginAt = DateTime.UtcNow;
                if (!string.IsNullOrEmpty(externalAuthDto.ProfilePictureUrl))
                {
                    user.ProfilePictureUrl = externalAuthDto.ProfilePictureUrl;
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Générer les tokens
            var accessToken = _tokenService.GenerateAccessToken(user.Id, user.Email);
            var refreshTokenString = _tokenService.GenerateRefreshToken(user.Id);

            // Stocker le refresh token en base de données
            var refreshToken = new RefreshToken
            {
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                CreatedByIp = ipAddress ?? "Unknown"
            };

            await _unitOfWork.RefreshTokens.AddAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenString,
                User = _mapper.Map<UserDto>(user)
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'authentification externe {Provider}", externalAuthDto.Provider);
            throw;
        }
    }

    public async Task<bool> RevokeTokenAsync(string refreshToken, string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var storedRefreshToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken, cancellationToken);
            
            if (storedRefreshToken == null || !storedRefreshToken.IsActive)
            {
                return false;
            }

            // Révoquer le token
            await _unitOfWork.RefreshTokens.RevokeTokenAsync(refreshToken, ipAddress, cancellationToken);
            
            _logger.LogInformation("Refresh token révoqué pour l'utilisateur {UserId}", storedRefreshToken.UserId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la révocation du token");
            throw;
        }
    }

    public async Task<bool> ConfirmEmailAsync(Guid userId, string confirmationToken, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // TODO: Valider le token de confirmation (à implémenter avec un système de tokens)
            // Pour l'instant, on confirme directement
            user.EmailConfirmed = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Email confirmé pour l'utilisateur : {Email}", user.Email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la confirmation de l'email pour l'utilisateur {UserId}", userId);
            throw;
        }
    }

    public async Task<UserDto?> GetCurrentUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<UserDto>(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération de l'utilisateur {UserId}", userId);
            throw;
        }
    }
}
