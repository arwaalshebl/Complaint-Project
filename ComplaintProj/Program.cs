using ComplaintProj;
using ComplaintProj.Data;
using ComplaintProj.Interceptors;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

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


//interceptor
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<AuditInterceptor>();

builder.Services.AddDbContext<AppDbContext>((sp, options) =>
{
    var interceptor = sp.GetRequiredService<AuditInterceptor>();

    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .AddInterceptors(interceptor);
});

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

    //create accounts
    async Task CreateTestUserAsync(string email, string password, string role)
    {
        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            var newUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(newUser, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(newUser, role);
                Console.WriteLine($"User {email} created with role {role}");
            }
        }
    }

    await CreateTestUserAsync("admin@gmail.com", "@Arwa123", "Admin");
    await CreateTestUserAsync("provider@gmail.com", "@Arwa123", "HealthcareProvider");
    await CreateTestUserAsync("services@gmail.com", "@Arwa123", "PatientServices");
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
    pattern: "{controller=Complaints}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
