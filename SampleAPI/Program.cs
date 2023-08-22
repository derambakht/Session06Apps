using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Core;
using Infrastructure.Utility;
using Application.Services;
using Application.IServices;
using Prometheus;
using Application.Services.Securrity;
using Application.IServices.Security;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SampleAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<SampleDbContext>(options => 
options.UseSqlServer(builder.Configuration.GetConnectionString("SampleConnectionString")));

builder.Services.AddSingleton(typeof(SampleDB));
builder.Services.AddSingleton(typeof(FileUtility));
builder.Services.AddSingleton(typeof(EncryptionUtility));

builder.Services.AddMiniProfiler(options => options.RouteBasePath = "/profiler").AddEntityFramework();

// builder.Services.AddScoped<ICompanyService, DapperCompanyService>();
// builder.Services.AddScoped<ICompanyService, EFCompanyService>();
// builder.Services.AddScoped<IAuthenticateService, AuthenticateService>();

builder.Services.AddApplicationService();
builder.Services.AddJWT();

builder.Services.AddMemoryCache();

// ===========================================
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseStaticFiles(new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(
                            Path.Combine(Directory.GetCurrentDirectory(), @"Media")),
                            RequestPath = new PathString("/Media")
    });

app.UseAuthentication();
app.UseAuthorization();

app.UseMiniProfiler();

app.UseMetricServer();
app.UseHttpMetrics();



//mini api
// app.MapGet("/PersonInf/{id}", () => {

// });

app.MapControllers();
app.MapMetrics();
app.Run();
