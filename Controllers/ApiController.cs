using insurance_company_partner_manager.Services;
using Microsoft.AspNetCore.Mvc;

namespace insurance_company_partner_manager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PartnerVipUpdatesController(InsurancePolicyService insurancePolicyService) : ControllerBase
    {

        private readonly InsurancePolicyService insurancePolicyService = insurancePolicyService;

        [HttpGet("stream")]
        public async Task Index()
        {
            Response.ContentType = "text/event-stream";

            TaskCompletionSource<bool> promise = new();

            async Task partnerUpdateCbAsync(string partnerUpdateJson)
            {
                await this.Response.WriteAsync($"data: {partnerUpdateJson}\n\n");
                await Response.Body.FlushAsync();
                promise.TrySetResult(true);
            }

            this.insurancePolicyService.SubscribeVipPartnerUpdates(partnerUpdateCbAsync);
            await promise.Task;
            this.insurancePolicyService.UnsubscribeVipPartnerUpdates(partnerUpdateCbAsync);
        }

    }
}