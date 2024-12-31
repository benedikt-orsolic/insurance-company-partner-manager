using System.Data.SQLite;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Models;


namespace insurance_company_partner_manager.Controllers
{
	public class InsurancePolicyController : Controller
	{
		private readonly SQLiteConnection connection = new (@"Data Source=/home/benedikt/Development/insurance-company-partner-manager/partner.db");
		public InsurancePolicyController () { }

		// GET: AddPolicy
		public IActionResult AddPolicy(string PartnerNumber)
		{
			InsurancePolicy policy = new() { PartnerNumber = PartnerNumber };
			return View(policy);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> AddPolicy([Bind("PolicyNumber,Amount,,PartnerNumber")] InsurancePolicy insurancePolicy)
		{
			this.connection.Open();

			IEnumerable<Partner> partnerList = await this.connection.QueryAsync<Partner>(
					"SELECT * FROM Partner WHERE (PartnerNumber = @PartnerNumber)", insurancePolicy);
			this.connection.Close();

			if(!partnerList.Any()) {
				ModelState.AddModelError("PartnerNumber", "Selected partner does not exist");
			}

			if (ModelState.IsValid)
			{
				this.connection.Open();
				var result = await this.connection.ExecuteAsync(
					"INSERT INTO InsurancePolicy (PolicyNumber, Amount, PartnerNumber) values (@PolicyNumber, @Amount, @PartnerNumber)", insurancePolicy);
				this.connection.Close();
				Console.WriteLine(result.ToString());
				return RedirectToAction("Index", "PartnerList");
			}

			return View(insurancePolicy);
		}

	}
}
