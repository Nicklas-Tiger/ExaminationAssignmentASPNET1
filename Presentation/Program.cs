using Microsoft.AspNetCore.Rewrite;
using Data;
using Business;
using Hubs;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddSignalR();

builder.Services.AddContexts(builder.Configuration.GetConnectionString("LocalDB")!);
builder.Services.AddLocalIdentity(builder.Configuration);

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/auth/login";     
    options.AccessDeniedPath = "/auth/denied"; 
});
builder.Services.AddRepositories(builder.Configuration);
builder.Services.AddServices(builder.Configuration);

var app = builder.Build();
app.UseHsts();
app.UseHttpsRedirection();

app.UseRewriter(new RewriteOptions().AddRedirect("^$", "/admin/overview"));
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.UseStaticFiles();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Overview}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapHub<NotificationHub>("/notificationHub");

app.Run();
