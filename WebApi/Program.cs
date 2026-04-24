using Data; // Tu namespace para servicios personalizados
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using Scalar.AspNetCore;
using System.Globalization;
using System.Text;
using WebApi;

// 1. CreateBuilder
var builder = WebApplication.CreateBuilder(args);

var cultureInfo = new CultureInfo("en-US");
var supportedCultures = new[] { cultureInfo };


builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    // Establecer la cultura por defecto
    options.DefaultRequestCulture = new RequestCulture(cultureInfo);

    // Forzar el uso de solo en-US en la lista de culturas soportadas
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

     
});


// 2. Configuración de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", p =>
        p.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});


// 3. Configuración de servicios
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });



// Autenticación JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
        ),
        ClockSkew = TimeSpan.Zero
    };
});

// 4. Servicios personalizados
builder.Services.AddScoped<dColor>();
builder.Services.AddScoped<dBrand>();
builder.Services.AddScoped<dContact>();
builder.Services.AddScoped<dModel>();
builder.Services.AddScoped<dVehicle>();
builder.Services.AddScoped<dPolicyType>();
builder.Services.AddScoped<dPolicy>();
builder.Services.AddScoped<dModule>();
builder.Services.AddScoped<dPart>();
builder.Services.AddScoped<dWarehouse>();
builder.Services.AddScoped<dZone>();
builder.Services.AddScoped<dLocation>();
builder.Services.AddScoped<dInventory>();
builder.Services.AddScoped<dService>();
builder.Services.AddScoped<dComment>();
builder.Services.AddScoped<dPayMethod>();
builder.Services.AddScoped<dAttachment>();
builder.Services.AddScoped<dSaleOrder>();
builder.Services.AddScoped<dLicense>();
builder.Services.AddScoped<dLaborTime>();
builder.Services.AddScoped<dPrintqueue>();
builder.Services.AddScoped<dSecurity>();
builder.Services.AddScoped<dAssessment>();
builder.Services.AddSingleton<RefreshTokenStore>();
builder.Services.AddScoped<dTemplate>();
builder.Services.AddScoped<dReporting>();
builder.Services.AddScoped<dPayment>();
builder.Services.AddScoped<dIntt>();
builder.Services.AddScoped<dInspection>();
builder.Services.AddScoped<dRate>();

builder.Services.AddHttpClient();
builder.Services.AddHostedService<WebApi.BackgroundServices.TasaBackgroundService>();

var app = builder.Build();


app.UseRouting();
app.UseCors("AllowAll");
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.StatusCode = 204; // No Content
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});

//*****************
app.MapOpenApi();
app.MapScalarApiReference(options =>
{

    //options.OpenApiRoutePattern = "/MCAExpenseWebApi/openapi/v1.json";
    options.OpenApiRoutePattern = "/openapi/{documentName}.json";
    options.WithTitle("WebApi");
    options.WithTheme(ScalarTheme.BluePlanet);


});
//*****************

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.UseRequestLocalization();


// 7. app run
app.Run();

// Clase para almacenar refresh tokens en memoria
public class RefreshTokenStore
{
    private readonly Dictionary<string, string> _refreshTokens = new();
    public void Save(string username, string refreshToken) =>
        _refreshTokens[username] = refreshToken;

    public string? Get(string username) =>
        _refreshTokens.TryGetValue(username, out var token) ? token : null;

    public void Remove(string username) =>
        _refreshTokens.Remove(username);
}



