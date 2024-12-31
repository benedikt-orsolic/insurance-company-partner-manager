namespace Models;

public class PartnerAdd()
{
    public EPartnerTypeId PartnerTypeId { get; set; } 
    public string FirstName { get; set; } 
    public string LastName { get; set; } 
    public string? Address { get; set; } 
    public ulong? CroatianPin { get; set; } 
    public bool IsForeign { get; set; } 
    public string ExternalCode { get; set; } 
    public string Gender { get; set; } 
    public string PartnerNumber { get; set; } 
    public string CreatedByUser { get; set; } 

    public override string ToString() {


		return $"FirstName: \t{this.FirstName}\n" +
		       $"LastName: \t{this.LastName}\n" + 
		       $"PartnerTypeId: \t{this.PartnerTypeId}\n" + 
		       $"Address: \t{this.Address}\n" + 
		       $"CroatianPin: \t{this.CroatianPin}\n" + 
		       $"IsForeign: \t{this.IsForeign}\n" + 
		       $"ExternalCode: \t{this.ExternalCode}\n" + 
		       $"Gender: \t{this.Gender}\n" + 
		       $"PartnerNumber: \t{this.PartnerNumber}\n" + 
		       $"CreatedByUser: \t{this.CreatedByUser}\n"; 

	}
}
