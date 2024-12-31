using insurance_company_partner_manager.Services;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddSingleton(builder.Configuration);
builder.Services.AddScoped<PartnerDbService, PartnerDbService>();
builder.Services.AddScoped<PartnerService, PartnerService>();
builder.Services.AddScoped<InsurancePolicyService, InsurancePolicyService>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PartnerList}/{action=Index}/{PartnerNumber?}"
);

app.Run();
