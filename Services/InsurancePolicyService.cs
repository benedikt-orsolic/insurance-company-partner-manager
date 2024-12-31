using System.Text.Json;
using Dapper;
using Models;

namespace insurance_company_partner_manager.Services;
public class InsurancePolicyService(PartnerDbService partnerDbService)
{
    private readonly HashSet<Func<string, Task>> vipPartnerUpdateCb = [];

    private readonly PartnerDbService _partnerDbService = partnerDbService;

    public async Task<IEnumerable<InsurancePolicyModel>> GetPolicyListByPartnerNumber(string partnerNumber)
    {
        this._partnerDbService.Connect();
        IEnumerable<InsurancePolicyModel> policyList = await this._partnerDbService.connection.QueryAsync<InsurancePolicyModel>(
                @"SELECT PolicyNumber, Amount FROM InsurancePolicy WHERE (PartnerNumber=@PartnerNumber)", new { PartnerNumber = partnerNumber });

        return policyList;
    }

    public async Task<bool> PolicyNumberExists(string policyNumber)
    {
        this._partnerDbService.Connect();
        IEnumerable<Partner> policyList = await this._partnerDbService.connection.QueryAsync<Partner>(
                "SELECT PolicyNumber FROM InsurancePolicy WHERE (PolicyNumber = @PolicyNumber)", new { PolicyNumber = policyNumber });

        return policyList.Any();
    }

    public async Task AddInsurancePolicy(InsurancePolicyModel insurancePolicy)
    {
        this._partnerDbService.Connect();
        await this._partnerDbService.connection.ExecuteAsync(
            "INSERT INTO InsurancePolicy (PolicyNumber, Amount, PartnerNumber) values (@PolicyNumber, @Amount, @PartnerNumber)", insurancePolicy);

        // Dispatch updated list of vip partners, this could only send updates for partnerNumber associated with insurancePolicy 
        IEnumerable<PartnerNumberModel> policyList = await this._partnerDbService.connection.QueryAsync<PartnerNumberModel>(
                @"SELECT PartnerNumber
                        FROM InsurancePolicy
                        GROUP BY PartnerNumber
                        HAVING Count(PartnerNumber) > 5 OR Sum(Amount) > 5000;
                ");


        string json = JsonSerializer.Serialize(policyList.Select(i => i.PartnerNumber));
        foreach (Func<string, Task> cb in vipPartnerUpdateCb)
        {
            await cb(json);
        }
    }

    public void SubscribeVipPartnerUpdates(Func<string, Task> cb)
    {
        vipPartnerUpdateCb.Add(cb);
    }

    public void UnsubscribeVipPartnerUpdates(Func<string, Task> cb)
    {
        vipPartnerUpdateCb.Remove(cb);
    }
}