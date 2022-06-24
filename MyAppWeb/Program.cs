using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using MyApp.DataAccessLayer.Infrastructure.IRepository;
using MyApp.DataAccessLayer.Infrastructure.Repository;
using MyApp.Models;
using MyApp.Models.Security;
using MyAppWeb.DataAccessLayer;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
// Add interface directory
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Add CustomClasswhichmanageotheradminoneadminmanageother
//builder.Services.AddSingleton<IAuthorizationHandler,
//        CanEditOnlyOtherAdminRolesAndClaimsHandler>();
// add identity in database
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    //set custom options on email
    options.Password.RequiredLength = 10;
    options.Password.RequiredUniqueChars = 3;
}).AddEntityFrameworkStores<ApplicationDbContext>();
//Add connnection string
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnecction"));
});
//authorize filter overall the controller

//builder.Services.AddAuthorization(options =>
//{
//    options.FallbackPolicy = new AuthorizationPolicyBuilder()
//        .RequireAuthenticatedUser()
//        .Build();
//});

builder.Services.AddControllers(config =>
{
    var policy = new AuthorizationPolicyBuilder()
                     .RequireAuthenticatedUser()
                     .Build();
    config.Filters.Add(new AuthorizeFilter(policy));
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("DeleteRolePolicy",
        policy => policy.RequireClaim("Delete Role")
                        .RequireClaim("Create Role"));

    options.AddPolicy("AdminRolePolicy",
       policy => policy.RequireRole("Admin","User"));

    //options.addpolicy("editrolepolicy",
    //policy => policy.requireclaim("edit role", "true")
    //  .requireclaim("admin")
    //  .requireclaim("super admin"));

    //Custom authorization policy
    //options.AddPolicy("EditRolePolicy",
    //    policy => policy.RequireAssertion(context =>
    //    context.User.IsInRole("Admin") &&
    //    context.User.HasClaim(claim => claim.Type == "Edit Role" && claim.Value == "true") ||
    //    context.User.IsInRole("Super Admin")
    //));

    //CustomeAdminRolesClaimsOnly one Admin manage
    options.AddPolicy("EditRolePolicy", policy =>
            policy.AddRequirements(new ManageAdminRolesAndClaimsRequirement()));
});
//
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{area=Admin}/{controller=Account}/{action=login}/{id?}");

app.Run();
