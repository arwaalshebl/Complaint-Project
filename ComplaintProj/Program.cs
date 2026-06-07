using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

using ComplaintProj.Data;
using ComplaintProj;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
                //.AddViewLocalization() //lang
                // .AddDataAnnotationsLocalization(options => {
                //             options.DataAnnotationLocalizerProvider = (type, factory) =>
                //           factory.Create(typeof(SharedResource));
                //         });


//Database
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));


//idintity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

////lang
//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
////lang
//var supportedCultures = new[] { "en-US", "ar-SA" };
//var localizationOptions = new RequestLocalizationOptions()
//    .SetDefaultCulture("en-US")
//    .AddSupportedCultures(supportedCultures)
//    .AddSupportedUICultures(supportedCultures);

var app = builder.Build();

//Roles
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

    string[] roleNames = { "Admin", "Patient", "PatientServices", "HealthcareProvider" };

    foreach (var roleName in roleNames)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
            Console.WriteLine($"Role {roleName} created");
        }
    }


}


// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
////lang
//app.UseRequestLocalization(localizationOptions);

app.UseHttpsRedirection();
app.UseRouting();
//idintity
app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Complaints}/{action=Create}/{id?}")
    .WithStaticAssets();


app.Run();
