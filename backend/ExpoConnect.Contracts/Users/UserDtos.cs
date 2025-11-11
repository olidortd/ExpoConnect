namespace ExpoConnect.Contracts.Users;

public sealed record UserResponse(
    string UserId,
    string Email,
    string? DisplayName,
    string? Phone,
    string? Company,
    string Role,
    bool IsActive,
    DateTime CreatedAt
);

public sealed record UserCreateRequest(
    string Email,
    string? DisplayName,
    string? Phone,
    string? Company,
    string Role,   // "Admin" | "Exhibitor" | "Visitor"
    string Password
);

public sealed record UserUpdateRequest(
    string? DisplayName,
    string? Phone,
    string? Company,
    string? Role,
    bool? IsActive
);

public sealed record PagedResult<T>(IReadOnlyList<T> Items, int Total, int Page, int PageSize);
