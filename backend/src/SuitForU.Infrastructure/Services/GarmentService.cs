using AutoMapper;
using Microsoft.Extensions.Logging;
using SuitForU.Application.DTOs.Common;
using SuitForU.Application.DTOs.Garments;
using SuitForU.Application.Interfaces;
using SuitForU.Domain.Entities;
using SuitForU.Domain.Interfaces;

namespace SuitForU.Infrastructure.Services;

public class GarmentService : IGarmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;
    private readonly IMapper _mapper;
    private readonly ILogger<GarmentService> _logger;

    public GarmentService(
        IUnitOfWork unitOfWork,
        IFileStorageService fileStorageService,
        IMapper mapper,
        ILogger<GarmentService> logger)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<PagedResult<GarmentDto>> GetAllGarmentsAsync(GarmentSearchDto searchDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var garments = await _unitOfWork.Garments.SearchGarmentsAsync(
                searchTerm: searchDto.SearchTerm,
                city: searchDto.City,
                maxPrice: searchDto.MaxPrice,
                type: searchDto.Type,
                size: searchDto.Size,
                pageNumber: searchDto.PageNumber,
                pageSize: searchDto.PageSize,
                cancellationToken: cancellationToken);

            var totalCount = await _unitOfWork.Garments.CountAsync(
                searchTerm: searchDto.SearchTerm,
                city: searchDto.City,
                maxPrice: searchDto.MaxPrice,
                type: searchDto.Type,
                size: searchDto.Size,
                cancellationToken: cancellationToken);

            var garmentDtos = _mapper.Map<List<GarmentDto>>(garments);

            return new PagedResult<GarmentDto>
            {
                Items = garmentDtos,
                TotalCount = totalCount,
                PageNumber = searchDto.PageNumber,
                PageSize = searchDto.PageSize
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des vêtements");
            throw;
        }
    }

    public async Task<GarmentDetailsDto?> GetGarmentByIdAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var garment = await _unitOfWork.Garments.GetGarmentWithDetailsAsync(id, cancellationToken);
            if (garment == null)
            {
                return null;
            }

            // Incrémenter le compteur de vues seulement si ce n'est pas le propriétaire
            if (!userId.HasValue || userId.Value != garment.OwnerId)
            {
                garment.ViewCount++;
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }

            var garmentDto = _mapper.Map<GarmentDetailsDto>(garment);
            return garmentDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération du vêtement {GarmentId}", id);
            throw;
        }
    }

    public async Task<GarmentDto> CreateGarmentAsync(Guid ownerId, CreateGarmentDto createDto, CancellationToken cancellationToken = default)
    {
        try
        {
            // Vérifier que l'utilisateur existe
            var owner = await _unitOfWork.Users.GetByIdAsync(ownerId);
            if (owner == null)
            {
                throw new InvalidOperationException("Utilisateur introuvable.");
            }

            // Créer le vêtement
            var garment = new Garment
            {
                OwnerId = ownerId,
                Title = createDto.Title,
                Description = createDto.Description,
                Type = createDto.Type,
                Condition = createDto.Condition,
                Size = createDto.Size,
                Brand = createDto.Brand,
                Color = createDto.Color,
                DailyPrice = createDto.DailyPrice,
                DepositAmount = createDto.DepositAmount,
                PickupAddress = createDto.PickupAddress,
                City = createDto.City,
                PostalCode = createDto.PostalCode,
                Country = createDto.Country,
                IsAvailable = true,
                ViewCount = 0
            };

            // TODO: Géocodage de l'adresse pour obtenir latitude/longitude
            // Pour l'instant, on laisse null
            garment.Latitude = null;
            garment.Longitude = null;

            await _unitOfWork.Garments.AddAsync(garment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Vêtement créé : {GarmentId} par {OwnerId}", garment.Id, ownerId);

            return _mapper.Map<GarmentDto>(garment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la création du vêtement par {OwnerId}", ownerId);
            throw;
        }
    }

    public async Task<GarmentDto> UpdateGarmentAsync(Guid id, Guid ownerId, UpdateGarmentDto updateDto, CancellationToken cancellationToken = default)
    {
        try
        {
            var garment = await _unitOfWork.Garments.GetByIdAsync(id);
            if (garment == null)
            {
                throw new InvalidOperationException("Vêtement introuvable.");
            }

            // Vérifier que l'utilisateur est bien le propriétaire
            if (garment.OwnerId != ownerId)
            {
                throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à modifier ce vêtement.");
            }

            // Vérifier qu'il n'y a pas de location active
            var hasActiveRentals = await _unitOfWork.Rentals.HasActiveRentalsAsync(id, cancellationToken);
            if (hasActiveRentals)
            {
                throw new InvalidOperationException("Impossible de modifier un vêtement avec des locations actives.");
            }

            // Mettre à jour les champs
            if (!string.IsNullOrEmpty(updateDto.Title))
                garment.Title = updateDto.Title;

            if (!string.IsNullOrEmpty(updateDto.Description))
                garment.Description = updateDto.Description;

            if (updateDto.DailyPrice.HasValue && updateDto.DailyPrice.Value > 0)
                garment.DailyPrice = updateDto.DailyPrice.Value;

            if (updateDto.DepositAmount.HasValue && updateDto.DepositAmount.Value >= 0)
                garment.DepositAmount = updateDto.DepositAmount.Value;

            if (updateDto.IsAvailable.HasValue)
                garment.IsAvailable = updateDto.IsAvailable.Value;

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Vêtement mis à jour : {GarmentId}", id);

            return _mapper.Map<GarmentDto>(garment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la mise à jour du vêtement {GarmentId}", id);
            throw;
        }
    }

    public async Task DeleteGarmentAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default)
    {
        try
        {
            var garment = await _unitOfWork.Garments.GetByIdAsync(id);
            if (garment == null)
            {
                throw new InvalidOperationException("Vêtement introuvable.");
            }

            // Vérifier que l'utilisateur est bien le propriétaire
            if (garment.OwnerId != ownerId)
            {
                throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à supprimer ce vêtement.");
            }

            // Vérifier qu'il n'y a pas de location active ou future
            var hasActiveOrFutureRentals = await _unitOfWork.Rentals.HasActiveOrFutureRentalsAsync(id, cancellationToken);
            if (hasActiveOrFutureRentals)
            {
                throw new InvalidOperationException("Impossible de supprimer un vêtement avec des locations actives ou futures.");
            }

            // Supprimer les images du stockage
            var allImages = await _unitOfWork.GarmentImages.GetAllAsync(cancellationToken);
            var images = allImages.Where(img => img.GarmentId == id).ToList();

            foreach (var image in images)
            {
                await _fileStorageService.DeleteFileAsync(image.ImageUrl);
            }

            // Soft delete
            garment.IsDeleted = true;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Vêtement supprimé : {GarmentId}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la suppression du vêtement {GarmentId}", id);
            throw;
        }
    }

    public async Task<string> UploadGarmentImageAsync(Guid garmentId, Guid ownerId, Stream imageStream, string fileName, CancellationToken cancellationToken = default)
    {
        try
        {
            var garment = await _unitOfWork.Garments.GetByIdAsync(garmentId);
            if (garment == null)
            {
                throw new InvalidOperationException("Vêtement introuvable.");
            }

            // Vérifier que l'utilisateur est bien le propriétaire
            if (garment.OwnerId != ownerId)
            {
                throw new UnauthorizedAccessException("Vous n'êtes pas autorisé à ajouter des images à ce vêtement.");
            }

            // Vérifier le nombre d'images (max 3)
            var allImages = await _unitOfWork.GarmentImages.GetAllAsync(cancellationToken);
            var existingImagesCount = allImages.Count(img => img.GarmentId == garmentId);

            if (existingImagesCount >= 3)
            {
                throw new InvalidOperationException("Nombre maximum d'images atteint (3 images maximum).");
            }

            // Upload de l'image
            var imageUrl = await _fileStorageService.UploadFileAsync(
                imageStream,
                fileName,
                "garments",
                cancellationToken);

            // Créer l'enregistrement de l'image
            var garmentImage = new GarmentImage
            {
                GarmentId = garmentId,
                ImageUrl = imageUrl,
                Order = existingImagesCount + 1,
                IsPrimary = existingImagesCount == 0 // La première image est la principale
            };

            await _unitOfWork.GarmentImages.AddAsync(garmentImage);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Image ajoutée au vêtement {GarmentId}", garmentId);

            return imageUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de l'upload de l'image pour le vêtement {GarmentId}", garmentId);
            throw;
        }
    }

    public async Task<List<GarmentDto>> GetUserGarmentsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var garments = await _unitOfWork.Garments.GetByOwnerIdAsync(userId, cancellationToken);
            return _mapper.Map<List<GarmentDto>>(garments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Erreur lors de la récupération des vêtements de l'utilisateur {UserId}", userId);
            throw;
        }
    }
}
