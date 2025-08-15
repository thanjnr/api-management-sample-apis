// Copyright (c) Microsoft Corporation. All Rights Reserved.
// Licensed under the MIT License.

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Todo.ClientApi;
using Todo.ClientApi.Services;

var builder = WebApplication.CreateBuilder(args);

/******************************************************************************************
**
** Add services to the container
*/

/*
** Application Insights logging
*/
builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAd")
    .EnableTokenAcquisitionToCallDownstreamApi()
    .AddInMemoryTokenCaches();

builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    // Set your custom IssuerSigningKey here
    // Example: using a SymmetricSecurityKey from a byte array
    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Convert.FromBase64String(builder.Configuration["InternalApiSettings:IssuerSigningKey"] ?? string.Empty));
    options.TokenValidationParameters.ValidAudience = builder.Configuration["InternalApiSettings:ValidAudience"];
});

/*
** CORS
*/
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

/*
** The Data Service.
*/
//builder.Services.AddScoped<ITodoDataService, TodoDataService>();

builder.Services.AddTransient<ITokenClientHelper, TokenClientHelper>();

/*
** Controllers.
*/
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.WriteIndented = true;
    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase, true));
});

/*
** Swagger / OpenAPI.
*/
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
** HttpClient.
*/
builder.Services.AddHttpClient();

/*
** InternalApiSettings.
*/
builder.Services.Configure<InternalApiSettings>(builder.Configuration.GetSection("InternalApiSettings"));

/******************************************************************************************
**
** Configure the HTTP Pipeline.
*/
var app = builder.Build();

/*
** CORS
*/
app.UseCors();

/*
** Add Swagger support.
*/
app.UseSwagger();
app.UseSwaggerUI();

/*
** Controllers.
*/
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

/************************************************************************************************
**
** Run the application.
*/
app.Run();
