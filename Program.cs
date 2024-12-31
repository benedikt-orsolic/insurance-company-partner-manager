var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
// var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
// builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

var app = builder.Build();

// app.UseHttpsRedirection();
// app.UseStaticFiles();
//
app.UseRouting();

// app.UseAuthorization();

app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=PartnerList}/{action=Index}/{PartnerNumber?}"
);

app.MapControllerRoute(
		name: "insurancePolicy",
		pattern: "{controller=InsurancePolicy}/{action=Index}"
);

app.Run();
