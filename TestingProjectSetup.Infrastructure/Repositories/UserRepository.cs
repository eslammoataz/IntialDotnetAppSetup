using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TestingProjectSetup.Application.Interfaces.Repositories;
using TestingProjectSetup.Domain.Models;
using TestingProjectSetup.Infrastructure.Data;

namespace TestingProjectSetup.Infrastructure.Repositories;

/// <summary>
/// User repository implementation
/// </summary>
public class UserRepository : Repository<ApplicationUser>, IUserRepository
{
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        : base(context)
    {
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetByPhoneNumberAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(u => u.PhoneNumber == phoneNumber, cancellationToken);
    }

    public async Task<ApplicationUser?> GetByTokenAsync(string token, CancellationToken cancellationToken = default)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.Value == token, cancellationToken);

        if (userToken is null) return null;

        return await _dbSet.FirstOrDefaultAsync(u => u.Id == userToken.UserId, cancellationToken);
    }

    public async Task SaveTokenAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        var userToken = new IdentityUserToken<string>
        {
            UserId = userId,
            LoginProvider = "CustomAuth",
            Name = "AccessToken",
            Value = token
        };

        await _context.UserTokens.AddAsync(userToken, cancellationToken);
    }

    public async Task<string?> GetTokenAsync(string userId, CancellationToken cancellationToken = default)
    {
        var token = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.LoginProvider == "CustomAuth", cancellationToken);
        return token?.Value;
    }

    public async Task RemoveTokenAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        var userToken = await _context.UserTokens
            .FirstOrDefaultAsync(t => t.UserId == userId && t.Value == token, cancellationToken);

        if (userToken is not null)
        {
            _context.UserTokens.Remove(userToken);
        }
    }

    public async Task<bool> ValidateTokenAsync(string userId, string token, CancellationToken cancellationToken = default)
    {
        return await _context.UserTokens
            .AnyAsync(t => t.UserId == userId && t.Value == token, cancellationToken);
    }

    public override async Task<ApplicationUser> AddAsync(ApplicationUser entity, CancellationToken cancellationToken = default)
    {
        var result = await _userManager.CreateAsync(entity);
        if (!result.Succeeded)
        {
            throw new InvalidOperationException($"Failed to create user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
        }
        return entity;
    }

    public override async Task UpdateAsync(ApplicationUser entity, CancellationToken cancellationToken = default)
    {
        await _userManager.UpdateAsync(entity);
    }

    public override async Task DeleteAsync(ApplicationUser entity, CancellationToken cancellationToken = default)
    {
        await _userManager.DeleteAsync(entity);
    }
}
