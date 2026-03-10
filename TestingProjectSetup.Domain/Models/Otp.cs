using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TestingProjectSetup.Domain.Models;

/// <summary>
/// OTP entity for phone verification
/// </summary>
public class Otp
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [StringLength(20)]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required]
    [StringLength(10)]
    public string Code { get; set; } = string.Empty;

    public DateTime Expiration { get; set; }

    public bool IsUsed { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsExpired() => DateTime.UtcNow > Expiration;

    public bool IsValid(string code) => !IsUsed && !IsExpired() && Code == code;
}
