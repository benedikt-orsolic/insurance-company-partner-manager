using insurance_company_partner_manager.Services;
using Microsoft.AspNetCore.Mvc;
using Models;

namespace insurance_company_partner_manager.Controllers
{
    public class InsurancePolicyController(PartnerService partnerService, InsurancePolicyService insurancePolicyService) : Controller
    {
        private readonly PartnerService partnerService = partnerService;
        private readonly InsurancePolicyService insurancePolicyService = insurancePolicyService;

        // GET: AddPolicy
        public IActionResult AddPolicy(string PartnerNumber)
        {
            InsurancePolicyModel policy = new() { PartnerNumber = PartnerNumber };
            return View(policy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPolicy([Bind("PolicyNumber,Amount,,PartnerNumber")] InsurancePolicyModel insurancePolicy)
        {
            bool partnerExists = await this.partnerService.PartnerExists(insurancePolicy.PartnerNumber ?? "");

            if (!partnerExists)
            {
                ModelState.AddModelError("PartnerNumber", "Selected partner does not exist");
            }

            bool policyNumberExists = await this.insurancePolicyService.PolicyNumberExists(insurancePolicy.PolicyNumber ?? "");

            if (policyNumberExists)
            {
                ModelState.AddModelError("PolicyNumber", "Selected policy number is already in use.");
            }

            if (ModelState.IsValid)
            {
                await this.insurancePolicyService.AddInsurancePolicy(insurancePolicy);
                return RedirectToAction("Index", "PartnerList");
            }

            return View(insurancePolicy);
        }

    }
}
