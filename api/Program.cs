using Serilog;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using API.Infrastructure;
using API.Messaging.Requests;
using API.Behavirous;
using API.Validators;
using API.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, configuration) =>
{
    configuration.ReadFrom.Configuration(context.Configuration);
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddGoogle(options =>
{
    options.ClientId = builder.Configuration["DOTNET_CLIENT_ID"] ?? throw new ArgumentNullException("Client Id is null");
    options.ClientSecret = builder.Configuration["DOTNET_CLIENT_SECRET"] ?? throw new ArgumentNullException("Client Secret is null");
    options.CallbackPath = new PathString(builder.Configuration["DOTNET_CALLBACK_PATH"]);
})
.AddCookie();

builder.Services.AddMediatR
(
    cfg => cfg
    .RegisterServicesFromAssembly(typeof(Program).Assembly)
    .AddOpenBehavior(typeof(LoggingPipelineBehaviour<,>))
    .AddOpenBehavior(typeof(ValidatorPipelineBehaviour<,>))
);
builder.Services.AddScoped<IValidator<PutFileRequest>, PutFileRequestValidator>();

builder.Services.AddS3FileStorage();

var app = builder.Build();

app.UseExceptionHandler("/Error");

app.UseAuthentication();

app.MapExceptionEndpoints();
app.MapAuthenticationEndpoints();
app.MapFileEndpoints();

app.Run();
