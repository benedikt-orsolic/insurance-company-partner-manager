
using Dapper;
using Models;
using Models.Dto;

namespace insurance_company_partner_manager.Services;
public class PartnerService(PartnerDbService partnerDbService)
{
    private readonly PartnerDbService partnerDbService = partnerDbService;

    public async Task<IList<PartnerListDisplayDto>> GetPartnerDisplayList()
    {
        this.partnerDbService.Connect();
        IEnumerable<PolicySumAndCountPartner> partnerList = await this.partnerDbService.connection.QueryAsync<PolicySumAndCountPartner>(
                @"SELECT 
						Partner.FirstName,
						Partner.LastName,
						Partner.PartnerNumber,
						Partner.PartnerTypeId,
						Partner.CreatedAtUtc,
						Partner.CreatedByUser,
						Partner.IsForeign,
						Partner.Gender,
						Sum(InsurancePolicy.Amount) As [TotalPolicyAmount],
						Count(InsurancePolicy.PolicyNumber) AS [PolicyCount]
					FROM Partner 
					LEFT JOIN InsurancePolicy ON Partner.PartnerNumber=InsurancePolicy.PartnerNumber
					GROUP BY Partner.PartnerNumber
					ORDER BY CreatedAtUtc DESC;");

        IList<PartnerListDisplayDto> displayList = partnerList
            .Select(p =>
                    new PartnerListDisplayDto(
                        hasPolicyTrigger: p.TotalPolicyAmount > 5_000.00 || p.PolicyCount > 5,
                        createdSuccess: false, // p.PartnerNumber.Equals(partnerNumber),
                        fullName: p.FirstName + " " + p.LastName,
                        partnerTypeId: p.PartnerTypeId,
                        croatianPin: p.CroatianPin,
                        createdAtUtc: DateTime.Parse(p.CreatedAtUtc),
                        isForeign: p.IsForeign,
                        gender: p.Gender,
                        partnerNumber: p.PartnerNumber
                    )
            )
            .ToList();

        return displayList;

    }

    public async Task<Partner?> FindPartnerByPartnerNumber(string partnerNumber)
    {
        this.partnerDbService.Connect();
        IEnumerable<Partner> partnerList = await this.partnerDbService.connection.QueryAsync<Partner>(
                @"SELECT * FROM Partner WHERE (PartnerNumber=@PartnerNumber)", new { PartnerNumber = partnerNumber });

        if (partnerList.Count() != 1)
        {
            throw new Exception("Something went wrong");
        }

        Partner? foundPartner = partnerList.FirstOrDefault();
        return foundPartner;
    }
    public async void AddPartner(Partner partner)
    {
        this.partnerDbService.Connect();
        await this.partnerDbService.connection.ExecuteAsync(
            @"INSERT INTO Partner 
						(FirstName,
						 LastName,
						 PartnerTypeId,
						 Address,
						 CroatianPin,
						 CreatedAtUtc,
						 IsForeign,
						 ExternalCode,
						 Gender,
						 PartnerNumber, 
						 CreatedByUser) 
					values (
						@FirstName,
						@LastName, 
						@PartnerTypeId, 
						@Address, 
						@CroatianPin, 
						@CreatedAtUtc, 
						@IsForeign,
						@ExternalCode, 
						@Gender, 
						@PartnerNumber, 
						@CreatedByUser)", partner);
    }

    public async Task<bool> PartnerExists(string partnerNumber)
    {
        this.partnerDbService.Connect();
        IEnumerable<Partner> partnerList = await this.partnerDbService.connection.QueryAsync<Partner>(
                "SELECT * FROM Partner WHERE (PartnerNumber = @PartnerNumber)", new { PartnerNumber = partnerNumber });

        return partnerList.Any();
    }
}