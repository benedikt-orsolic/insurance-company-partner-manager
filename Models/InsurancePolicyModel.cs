using System.ComponentModel.DataAnnotations;
namespace Models;

public class InsurancePolicyModel
{
	[Required]
	[Length(10, 15)]
	public string? PolicyNumber { get; set; }


	[Required]
	public double Amount { get; set; }


	[Required]
	[RegularExpression(@"^[0-9]{20}$", ErrorMessage = "Invalid partner number. Must be numeric with exactly 20 digits.")]
	public string? PartnerNumber { get; set; }
}
