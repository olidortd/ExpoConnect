using ExpoConnect.Contracts.Users;

namespace ExpoConnect.Application.Interfaces;

public interface IUsersService
{
    Task<PagedResult<UserResponse>> QueryAsync(string? q, int page, int pageSize, string? sort, CancellationToken ct);
    Task<UserResponse?> GetAsync(string userId, CancellationToken ct);
    Task<UserResponse> CreateAsync(UserCreateRequest req, CancellationToken ct);
    Task<UserResponse> UpdateAsync(string userId, UserUpdateRequest req, CancellationToken ct);
    Task DeleteAsync(string userId, CancellationToken ct); // soft-delete: סימון Inactive
}
