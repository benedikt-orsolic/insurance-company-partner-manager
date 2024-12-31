using System.ComponentModel.DataAnnotations;
using Models.Enum;
namespace Models;

public class Partner
{
    [Required]
    [Length(2, 255)]
    public string FirstName { get; set; } = "";

    [Required]
    [Length(2, 255)]
    public string LastName { get; set; } = "";

    public string? Address { get; set; }

    [Required]
    [RegularExpression(@"^[0-9]{20}$", ErrorMessage = "Invalid partner number. Must be numeric with exactly 20 digits.")]
    public string PartnerNumber { get; set; } = "";

    public ulong? CroatianPin { get; set; } 

    [Required]
    public EPartnerTypeId PartnerTypeId { get; set; } 

    [Required]
    public string CreatedAtUtc { get; set; } = "";

    [Required]
    [EmailAddress]
    [MaxLength(255)]
    public string CreatedByUser { get; set; } = "";

    [Required]
    public bool IsForeign { get; set; } 

    [Length(10, 20)]
    public string? ExternalCode { get; set; } 

    [Required]
    [RegularExpression(@"(M|F|N)")]
    public string Gender { get; set; } = "N";
}
