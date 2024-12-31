using System.ComponentModel.DataAnnotations;
using insurance_company_partner_manager.Services;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.Dto;

namespace insurance_company_partner_manager.Controllers
{
    public class PartnerListController(PartnerService partnerService, InsurancePolicyService insurancePolicyService) : Controller
    {

        // GET: PartnerList
        public async Task<IActionResult> Index(string partnerNumber)
        {
            IList<PartnerListDisplayDto> displayList = await partnerService.GetPartnerDisplayList();
            PartnerListDisplayDto? lastAddedPartner = displayList.FirstOrDefault((item) => item.PartnerNumber == partnerNumber);
            if (lastAddedPartner != null)
            {
                lastAddedPartner.CreatedSuccess = true;
            }

            return View(displayList);
        }


        public IActionResult AddPartner()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddPartner([Bind("FirstName,LastName,Address,CroatianPin,PartnerTypeId,IsForeign,ExternalCode,Gender,PartnerNumber,CreatedByUser")] Partner partner)
        {
            string currentTime = DateTime.UtcNow.ToString("s", System.Globalization.CultureInfo.InvariantCulture);
            partner.CreatedAtUtc = currentTime;

            ModelState.Clear();
            TryValidateModel(partner);

            if (ModelState.IsValid)
            {
                partnerService.AddPartner(partner);
                return RedirectToAction(nameof(Index), new { partnerNumber = partner.PartnerNumber });
            }

            return View(partner);
        }



        public async Task<IActionResult> PartnerDetails(string? PartnerNumber)
        {
            if (PartnerNumber == null)
            {
                throw new Exception("partner not found");
            }

            Partner p = await partnerService.FindPartnerByPartnerNumber(PartnerNumber) ?? throw new Exception("partner not found");

            IEnumerable<InsurancePolicyModel> policyList = await insurancePolicyService.GetPolicyListByPartnerNumber(PartnerNumber);

            PartnerDetailsDisplayDto partnerDisplay = new(
                fullName: p.FirstName + " " + p.LastName,
                partnerTypeId: p.PartnerTypeId,
                croatianPin: p.CroatianPin,
                createdAtUtc: DateTime.Parse(p.CreatedAtUtc),
                isForeign: p.IsForeign,
                gender: p.Gender,
                partnerNumber: p.PartnerNumber,
                insurancePolicyList: policyList,
                externalCode: p.ExternalCode,
                createdByUser: p.CreatedByUser
            );


            return View(partnerDisplay);
        }

    }
}
