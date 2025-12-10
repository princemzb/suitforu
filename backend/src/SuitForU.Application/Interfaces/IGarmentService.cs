using SuitForU.Application.DTOs.Common;
using SuitForU.Application.DTOs.Garments;

namespace SuitForU.Application.Interfaces;

public interface IGarmentService
{
    Task<PagedResult<GarmentDto>> GetAllGarmentsAsync(GarmentSearchDto searchDto, CancellationToken cancellationToken = default);
    Task<GarmentDetailsDto?> GetGarmentByIdAsync(Guid id, Guid? userId, CancellationToken cancellationToken = default);
    Task<GarmentDto> CreateGarmentAsync(Guid ownerId, CreateGarmentDto createDto, CancellationToken cancellationToken = default);
    Task<GarmentDto> UpdateGarmentAsync(Guid id, Guid ownerId, UpdateGarmentDto updateDto, CancellationToken cancellationToken = default);
    Task DeleteGarmentAsync(Guid id, Guid ownerId, CancellationToken cancellationToken = default);
    Task<string> UploadGarmentImageAsync(Guid garmentId, Guid ownerId, Stream imageStream, string fileName, CancellationToken cancellationToken = default);
    Task<List<GarmentDto>> GetUserGarmentsAsync(Guid userId, CancellationToken cancellationToken = default);
}
