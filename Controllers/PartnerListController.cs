using System.Data.SQLite;
using System.ComponentModel.DataAnnotations;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace insurance_company_partner_manager.Controllers
{
	public class PartnerListController : Controller
	{
		private readonly SQLiteConnection connection = new(@"Data Source=/home/benedikt/Development/insurance-company-partner-manager/partner.db");
		public PartnerListController() { }

		// GET: PartnerList
		public async Task<IActionResult> Index(string partnerNumber)
		{
			this.connection.Open();
			IEnumerable<PolicySumAndCountPartner> partnerList = await this.connection.QueryAsync<PolicySumAndCountPartner>(
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
			this.connection.Close();

			IList<PartnerDisplay> displayList = partnerList
				.Select(p => 
						new PartnerDisplay (
							hasPolicyTrigger : p.TotalPolicyAmount > 5_000.00 || p.PolicyCount > 5,
							createdSuccess : p.PartnerNumber.Equals(partnerNumber),
							fullName : p.FirstName + " " + p.LastName,
							partnerTypeId : p.PartnerTypeId,
							croatianPin : p.CroatianPin,
							createdAtUtc : DateTime.Parse(p.CreatedAtUtc),
							isForeign : p.IsForeign,
							gender : p.Gender,
							partnerNumber : p.PartnerNumber
                        )
				)
				.ToList();

			return View(displayList);
		}


		public IActionResult AddPartner()
		{
		    return View();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPartner([Bind("FirstName,LastName,Address,CroatianPin,PartnerTypeId,IsForeign,ExternalCode,Gender,PartnerNumber,CreatedByUser")] PartnerAdd partner)
		{
			string currentTime = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            // Console.WriteLine(partner.ToString(), currentTime);

            Partner partnerForDb = new Partner{
                FirstName= partner.FirstName,
				LastName = partner.LastName,
				PartnerTypeId = partner.PartnerTypeId,
				Address = partner.Address,
				CroatianPin = partner.CroatianPin,
				CreatedAtUtc = currentTime,
				IsForeign = partner.IsForeign,
				ExternalCode = partner.ExternalCode,
				Gender = partner.Gender,
				PartnerNumber = partner.PartnerNumber,
				CreatedByUser = partner.CreatedByUser
			};

			IList<ValidationResult> validationErrors = new List<ValidationResult>();
			ValidationContext partnerForDbValidationContext = new(partnerForDb);

			Validator.TryValidateObject(
					partnerForDb,
					partnerForDbValidationContext, 
					validationErrors, 
					validateAllProperties: true
				);

			foreach(ValidationResult error in validationErrors){
				if(error.ErrorMessage != null) {
						foreach(string member in error.MemberNames){
							ModelState.AddModelError(member, error.ErrorMessage);
					}
				}
			}

			if (ModelState.IsValid)
			{
				this.connection.Open();
				await this.connection.ExecuteAsync(
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
						@CreatedByUser)", partnerForDb);
				this.connection.Close();
				return RedirectToAction(nameof(Index), new { partnerNumber = partnerForDb.PartnerNumber});
			}

			return View(partner);
		}



		public async Task<IActionResult> PartnerDetails(string? PartnerNumber)
		{
			this.connection.Open();
			IEnumerable<Partner> partnerList = await this.connection.QueryAsync<Partner>(
					@"SELECT * FROM Partner WHERE (PartnerNumber=@PartnerNumber)", new { PartnerNumber } );
			this.connection.Close();

			if(partnerList.Count() != 1){
				throw new Exception("Something went wrong");
			}

			Partner? p  = partnerList.FirstOrDefault() ?? throw new Exception("partner not found");

            this.connection.Open();
			IEnumerable<InsurancePolicy> policyList = await this.connection.QueryAsync<InsurancePolicy>(
					@"SELECT PolicyNumber, Amount FROM InsurancePolicy WHERE (PartnerNumber=@PartnerNumber)", new { PartnerNumber });
			this.connection.Close();

            PartnerDetailsDisplay partnerDisplay = new(
				fullName : p.FirstName + " " + p.LastName,
				partnerTypeId : p.PartnerTypeId,
				croatianPin : p.CroatianPin,
				createdAtUtc : DateTime.Parse(p.CreatedAtUtc),
				isForeign : p.IsForeign,
				gender : p.Gender,
				partnerNumber : p.PartnerNumber,
				insurancePolicyList : policyList,
				externalCode : p.ExternalCode,
				createdByUser : p.CreatedByUser
			);


			return View(partnerDisplay);
		}

	}


}
