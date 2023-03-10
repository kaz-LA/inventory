using System.Net.NetworkInformation;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;
using Aine.Inventory.Api;
using Aine.Inventory.Core.DomainEventHandlers;
using Aine.Inventory.Infrastructure;
using Aine.Inventory.Infrastructure.Data;
using Aine.Inventory.SharedKernel;
using Ardalis.ListStartupServices;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using FastEndpoints;
using FastEndpoints.ApiExplorer;
using FastEndpoints.Security;
//using FastEndpoints.Swagger.Swashbuckle;
using FastEndpoints.Swagger;
using MediatR;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;

const string DESCRIPTION = "Aine Inventory Api 1.0";

var builder = WebApplication.CreateBuilder(args);
{

  builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

  builder.Host.UseSerilog((_, config) => config.ReadFrom.Configuration(builder.Configuration));

  builder.Services.Configure<CookiePolicyOptions>(options =>
  {
    options.CheckConsentNeeded = context => true;
    options.MinimumSameSitePolicy = SameSiteMode.None;
  });

  //builder.AddJwtAuthentication();
  //builder.Services.AddAuthorization();

  builder.Services.AddMediatR(typeof(Program));
  
  string? connectionString = builder.Configuration.GetConnectionString(builder.Configuration["ConnectionString"]);

  builder.Services.AddDbContext<AppDbContext>(
    builder =>
      builder.UseSqlite(connectionString!)
        .LogTo(Console.WriteLine)
        .EnableDetailedErrors()
        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
  );

  builder.Services.AddCors(options =>
  {
    options.AddDefaultPolicy(builder =>
        builder
        .AllowAnyOrigin()
        .AllowAnyHeader()
        .AllowAnyMethod()
        .WithExposedHeaders("Content-Disposition")
        );
  });
  builder.Services.AddControllers();
  //builder.Services.AddControllersWithViews().AddNewtonsoftJson();
  //builder.Services.AddRazorPages();
  builder.Services.AddFastEndpoints();
  builder.Services.AddJWTBearerAuth(builder.Configuration["Jwt:Key"]);  // FastEndpoints
  builder.Services.AddSwaggerDoc(settings =>
  {
    settings.Title = DESCRIPTION;
    settings.Version = "v1";
  });
  builder.Services.AddFastEndpointsApiExplorer();
  builder.Services.AddSwaggerGen(options =>
  {
    options.SwaggerDoc("v1", new OpenApiInfo { Title = DESCRIPTION, Version = "v1" });
    options.EnableAnnotations();
    //options.OperationFilter<FastEndpointsOperationFilter>();
    options.AddApiSecurityDefinition();
  });

  // add list services for diagnostic purposes - see https://github.com/ardalis/AspNetCoreStartupServices
  builder.Services.Configure<ServiceConfig>(config =>
  {
    config.Services = new List<ServiceDescriptor>(builder.Services);

    // optional - default path to view services is /listallservices - recommended to choose your own path
    config.Path = "/listservices";
  });

  builder.Host.ConfigureContainer<ContainerBuilder>(containerBuilder =>
  {
    // in assembly holding the Domain Events
    containerBuilder.RegisterAssemblyTypes(typeof(PriceUpdatedDomainEventHandler)
                                   .GetTypeInfo().Assembly)
                                     .AsClosedTypesOf(typeof(INotificationHandler<>));
    containerBuilder.RegisterAllTypesFromAssembly(typeof(SharedKernelMarker).Assembly, builder.Environment.EnvironmentName);
    containerBuilder.RegisterAllTypesFromAssembly(typeof(InfrastructureMarker).Assembly, builder.Environment.EnvironmentName);
    containerBuilder.RegisterAllTypesFromAssembly(typeof(CoreMarker).Assembly, builder.Environment.EnvironmentName);
    containerBuilder.RegisterAllTypesFromAssembly(typeof(Program).Assembly, builder.Environment.EnvironmentName);
  });

  builder.Services.Configure<JsonOptions>(options =>
  {
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    //options.SerializerOptions.IgnoreReadOnlyProperties = true;
  });

  builder.Services.Configure<FormOptions>(options =>
  {
    // Set the limit to 2 MB
    // options.MultipartBodyLengthLimit = 2 * 1024 * 1024;
  });
}

var app = builder.Build();
{
  //app.UsePathBase("/aine-inventory-api");

  app.UseExceptionHandler(a => a.Run(ErrorHandler.HandleException));
  app.UseHsts();
  app.UseStaticFiles();

  //if (app.Environment.IsDevelopment())
  //{
  //  app.UseDeveloperExceptionPage();
  //  app.UseShowAllServicesMiddleware();
  //}
  //else
  //{
  //  app.UseExceptionHandler("/Home/Error");
  //  app.UseHsts();
  //}
  app.UseRouting();
  app.UseCors();
  app.UseAuthentication();
  app.UseAuthorization();
  app.UseFastEndpoints(c =>
  {
    c.Serializer.Options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    c.Endpoints.RoutePrefix = "api";
  });

  app.UseHttpsRedirection();
  app.UseStaticFiles();
  app.UseCookiePolicy();

  // Enable middleware to serve generated Swagger as a JSON endpoint.
  app.UseSwaggerGen(); //SwaggerBuilderExtensions.UseSwagger(app);   //app.UseSwagger();

  // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
  app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", DESCRIPTION));
  //app.UseSwaggerGen(); //add this

  //app.MapDefaultControllerRoute();
  //app.MapRazorPages();
  app.MapControllers();

}

app.Run();

