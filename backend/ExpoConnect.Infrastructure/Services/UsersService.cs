using ExpoConnect.Application.Interfaces;
using ExpoConnect.Contracts.Users;
using ExpoConnect.Domain;
using ExpoConnect.Domain.Users;
using ExpoConnect.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ExpoConnect.Infrastructure.Services;

public sealed class UsersService : IUsersService
{
    private readonly AppDbContext _db;

    public UsersService(AppDbContext db) => _db = db;

    public async Task<PagedResult<UserResponse>> QueryAsync(string? q, int page, int pageSize, string? sort, CancellationToken ct)
    {
        var query = _db.Users.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(q))
        {
            var term = q.Trim().ToLowerInvariant();
            query = query.Where(u =>
                u.Email.ToLower().Contains(term) ||
                (u.DisplayName != null && u.DisplayName.ToLower().Contains(term)) ||
                (u.Company != null && u.Company.ToLower().Contains(term)));
        }

        query = sort?.ToLowerInvariant() switch
        {
            "email" => query.OrderBy(u => u.Email),
            "-email" => query.OrderByDescending(u => u.Email),
            "created" => query.OrderBy(u => u.CreatedAt),
            "-created" => query.OrderByDescending(u => u.CreatedAt),
            "displayname" => query.OrderBy(u => u.DisplayName),
            "-displayname" => query.OrderByDescending(u => u.DisplayName),
            _ => query.OrderByDescending(u => u.CreatedAt)
        };

        var total = await query.CountAsync(ct);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(u => new UserResponse(
                u.UserId, u.Email, u.DisplayName, u.Phone, u.Company,
                u.Role.ToString(), u.IsActive, u.CreatedAt))
            .ToListAsync(ct);

        return new PagedResult<UserResponse>(items, total, page, pageSize);
    }

    public async Task<UserResponse?> GetAsync(string userId, CancellationToken ct)
        => await _db.Users.AsNoTracking()
            .Where(u => u.UserId == userId)
            .Select(u => new UserResponse(
                u.UserId, u.Email, u.DisplayName, u.Phone, u.Company,
                u.Role.ToString(), u.IsActive, u.CreatedAt))
            .SingleOrDefaultAsync(ct);

    public async Task<UserResponse> CreateAsync(UserCreateRequest req, CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();
        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
            throw new InvalidOperationException("Email already exists.");

        var role = Enum.TryParse<UserRole>(req.Role, true, out var r) ? r : UserRole.visitor;

        var user = new User
        {
            Email = email,
            DisplayName = req.DisplayName,
            Phone = req.Phone,
            Company = req.Company,
            Role = role,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(req.Password, 11)
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        return new UserResponse(
            user.UserId, user.Email, user.DisplayName, user.Phone, user.Company,
            user.Role.ToString(), user.IsActive, user.CreatedAt);
    }

    public async Task<UserResponse> UpdateAsync(string userId, UserUpdateRequest req, CancellationToken ct)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.UserId == userId, ct)
                   ?? throw new KeyNotFoundException("User not found.");

        if (req.DisplayName is not null) user.DisplayName = req.DisplayName;
        if (req.Phone is not null) user.Phone = req.Phone;
        if (req.Company is not null) user.Company = req.Company;

        if (req.Role is not null)
        {
            if (Enum.TryParse<UserRole>(req.Role, true, out var role))
                user.Role = role;
            else
                throw new ArgumentException("Invalid role value.");
        }

        if (req.IsActive is not null) user.IsActive = req.IsActive.Value;

        await _db.SaveChangesAsync(ct);

        return new UserResponse(
            user.UserId, user.Email, user.DisplayName, user.Phone, user.Company,
            user.Role.ToString(), user.IsActive, user.CreatedAt);
    }

    public async Task DeleteAsync(string userId, CancellationToken ct)
    {
        var user = await _db.Users.SingleOrDefaultAsync(u => u.UserId == userId, ct)
                   ?? throw new KeyNotFoundException("User not found.");

        user.IsActive = false;
        await _db.SaveChangesAsync(ct);
    }
}
