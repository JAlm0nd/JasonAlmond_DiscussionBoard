using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using JasonAlmond_DiscussionBoard.Data;
using JasonAlmond_DiscussionBoard.Models;
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

builder.Services.AddScoped<DiscussionThreadService>();
builder.Services.AddScoped<IRepo<DiscussionThread>, DiscussionThreadRepo>();
builder.Services.AddScoped<IRepo<Post>, PostRepo>();
builder.Services.AddScoped<PostService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapRazorPages();

app.Run();