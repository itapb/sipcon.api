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

    // MUY IMPORTANTE: Cambiar el orden de los proveedores. 
    // Usaremos un proveedor que no se base en el encabezado del navegador.
    // Aunque no está FixedRequestCultureProvider, al configurar DefaultRequestCulture
    // y limitar las SupportedCultures, se logra el efecto deseado.

    // Opcionalmente, puedes eliminar todos los proveedores y confiar en DefaultRequestCulture
    // options.RequestCultureProviders.Clear(); 

    // Si usas un proveedor que se basa en la Query String (opcional)
    // options.RequestCultureProviders.Insert(0, new QueryStringRequestCultureProvider()); 
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
//builder.Services.AddSwaggerGen();

builder.Services.AddOpenApi("v1", options => { options.AddDocumentTransformer<BearerSecuritySchemeTransformer>(); });

// Swagger con seguridad Bearer
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SIPCON", Version = "v1" });
//    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        Scheme = "bearer",
//        BearerFormat = "JWT",
//        In = ParameterLocation.Header,
//        Description = "Ejemplo: \"Bearer eyJhbGciOiJIUzI1NiIs...\""
//    });
//    //c.AddSecurityRequirement(new OpenApiSecurityRequirement
//    //{
//    //    {
//    //        new OpenApiSecurityScheme
//    //        {
//    //            Reference = new OpenApiReference
//    //            {
//    //                Type = ReferenceType.SecurityScheme,
//    //                Id = "Bearer"
//    //            }
//    //        },
//    //        new string[] {}
//    //    }
//    //});
//});


//*****************************************

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
builder.Services.AddSingleton<dColor>();
builder.Services.AddSingleton<dFeatureType>();
builder.Services.AddSingleton<dArea>();
builder.Services.AddSingleton<dFase>();
builder.Services.AddSingleton<dBrand>();
builder.Services.AddSingleton<dContact>();
builder.Services.AddSingleton<dModel>();
builder.Services.AddSingleton<dVehicle>();
builder.Services.AddSingleton<dPolicyType>();
builder.Services.AddSingleton<dPolicy>();
builder.Services.AddSingleton<dModule>();
builder.Services.AddSingleton<dPart>();
builder.Services.AddSingleton<dWarehouse>();
builder.Services.AddSingleton<dZone>();
builder.Services.AddSingleton<dLocation>();
builder.Services.AddSingleton<dInventory>();
builder.Services.AddSingleton<dService>();
builder.Services.AddSingleton<dComment>();
builder.Services.AddSingleton<dPayMethod>();
builder.Services.AddSingleton<dAttachment>();
builder.Services.AddSingleton<dSaleOrder>();
builder.Services.AddSingleton<dLicense>();
builder.Services.AddSingleton<dLaborTime>();
builder.Services.AddSingleton<dPrintqueue>();
builder.Services.AddSingleton<dSecurity>();
builder.Services.AddSingleton<dAssessment>();
builder.Services.AddSingleton<RefreshTokenStore>();
builder.Services.AddSingleton<dTemplate>();
builder.Services.AddSingleton<dReporting>();
// 5. crear app
var app = builder.Build();

// 6. Use. ORDEN: UseSwagger,UseSwaggerUI,UseRouting,UseCors,Use,UseAuthentication,UseAuthorization,MapControllers
//app.UseSwagger();
//app.UseSwaggerUI();


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
    //options.HideSidebar();
    //options.Servers = [new ScalarServer("/MCAExpenseWebApi")];

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



