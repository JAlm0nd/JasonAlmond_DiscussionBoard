using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JasonAlmond_DiscussionBoard.Data;
using JasonAlmond_DiscussionBoard.Helpers;
using JasonAlmond_DiscussionBoard.Models;
using JasonAlmond_DiscussionBoard.Pages;
using JasonAlmond_DiscussionBoard.Repos;
using JasonAlmond_DiscussionBoard.Services;
using JasonAlmond.DiscussionBoard.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                       throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn
        .RequireConfirmedAccount = !builder.Environment.IsDevelopment())
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(PolicyTypes.IsAdmin, policy => policy.RequireClaim(PolicyTypes.IsAdmin, PolicyValues.True));
    options.AddPolicy(PolicyTypes.IsModerator, policy => policy.RequireAssertion(context =>
        context.User.HasClaim(c => c.Type.Equals(PolicyTypes.IsModerator) && c.Value.Equals(PolicyValues.True))
        ||
        context.User.HasClaim(c => c.Type.Equals(PolicyTypes.IsAdmin) && c.Value.Equals(PolicyValues.True))));
});
//Needed for layout testing
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<DiscussionThreadService>();
builder.Services.AddScoped<IRepo<DiscussionThread>, DiscussionThreadRepo>();
builder.Services.AddScoped<IRepo<Post>, PostRepo>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<ApplicationUserService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error"); // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();