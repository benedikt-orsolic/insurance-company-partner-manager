using Models.Enum;

namespace Models.Dto;

public class PartnerListDisplayDto(bool hasPolicyTrigger, EPartnerTypeId partnerTypeId, bool createdSuccess, string fullName, DateTime createdAtUtc, bool isForeign, string gender, string partnerNumber, ulong? croatianPin = null)
{
    public bool HasPolicyTrigger { get; set; } = hasPolicyTrigger;
    public EPartnerTypeId PartnerTypeId { get; set; } = partnerTypeId;
    public bool CreatedSuccess { get; set; } = createdSuccess;
    public string FullName { get; set; } = fullName;
    //public string Address { get; set; } = address;
    public ulong? CroatianPin { get; set; } = croatianPin;
    public DateTime CreatedAtUtc { get; set; } = createdAtUtc;
    public bool IsForeign { get; set; } = isForeign;
    // public string ExternalCode { get; set; } = externalCode;
    public string Gender { get; set; } = gender;
    public string PartnerNumber { get; set; } = partnerNumber;
}
