using Microsoft.EntityFrameworkCore;
using TestingProjectSetup.Application.Interfaces.Repositories;
using TestingProjectSetup.Domain.Models;
using TestingProjectSetup.Infrastructure.Data;

namespace TestingProjectSetup.Infrastructure.Repositories;

/// <summary>
/// OTP repository implementation
/// </summary>
public class OtpRepository : Repository<Otp>, IOtpRepository
{
    public OtpRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<Otp?> GetValidOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        return await _dbSet
            .Where(o => o.PhoneNumber == phoneNumber && !o.IsUsed && o.Expiration > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public Task InvalidateOtpAsync(Otp otp, CancellationToken cancellationToken = default)
    {
        otp.IsUsed = true;
        _dbSet.Update(otp);
        return Task.CompletedTask;
    }

    public async Task InvalidateAllOtpsForPhoneAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var otps = await _dbSet
            .Where(o => o.PhoneNumber == phoneNumber && !o.IsUsed)
            .ToListAsync(cancellationToken);

        foreach (var otp in otps)
        {
            otp.IsUsed = true;
        }
    }
}
