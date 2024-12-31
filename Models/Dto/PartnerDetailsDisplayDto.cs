using Models.Enum;
using Models;
namespace Models.Dto;

class PartnerDetailsDisplayDto(
    string fullName,
    EPartnerTypeId partnerTypeId,
    DateTime createdAtUtc,
    bool isForeign,
    string gender,
    string partnerNumber,
    IEnumerable<InsurancePolicyModel> insurancePolicyList,
    string createdByUser,
    string? address = null,
    ulong? croatianPin = null,
    string? externalCode = null
    )
{
    public EPartnerTypeId PartnerTypeId { get; set; } = partnerTypeId;
    public string FullName { get; set; } = fullName;
    public string? Address { get; set; } = address;
    public ulong? CroatianPin { get; set; } = croatianPin;
    public DateTime CreatedAtUtc { get; set; } = createdAtUtc;
    public bool IsForeign { get; set; } = isForeign;
    public string? ExternalCode { get; set; } = externalCode;
    public string Gender { get; set; } = gender;
    public string PartnerNumber { get; set; } = partnerNumber;
    public string CreatedByUser { get; set; } = createdByUser;
    public IEnumerable<InsurancePolicyModel> InsurancePolicyList { get; set; } = insurancePolicyList;
}
