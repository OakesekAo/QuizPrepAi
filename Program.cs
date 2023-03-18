using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using QuizPrepAi.Data;
using QuizPrepAi.Helpers;
using QuizPrepAi.Models;
using QuizPrepAi.Models.Settings;
using QuizPrepAi.Services;
using QuizPrepAi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = ConnectionHelper.GetConnectionString(builder.Configuration);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddDefaultIdentity<QPUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ApplicationDbContext>();
builder.Services.AddControllersWithViews();

builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

Console.WriteLine(builder.Configuration["AppSettings:QuizPrepAiSettings:OpenAiAPIKey"]);

//Custom Services
builder.Services.AddScoped<IQuizService, QuizService>();

var app = builder.Build();
var scope = app.Services.CreateScope();
// get the database update with the latest migrations
await DataHelper.ManageDataAsync(scope.ServiceProvider);



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
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
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
