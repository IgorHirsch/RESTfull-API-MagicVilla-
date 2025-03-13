
using MagicVilla_VillaAPI;
using MagicVilla_VillaAPI.Data;
using MagicVilla_VillaAPI.Repository;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);





builder.Services.AddDbContext<ApplicationDbContext>(option => { option.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));});
builder.Services.AddAutoMapper(typeof(MappingConfig));
builder.Services.AddControllers(option => {/*option.ReturnHttpNotAcceptable=true; */}).AddNewtonsoftJson().AddXmlDataContractSerializerFormatters();
builder.Services.AddScoped<IVillaRepository, VillaRepository>();
builder.Services.AddScoped<IVillaNumberRepository, VillaNumberRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

var key = builder.Configuration.GetValue<string>("ApiSettings:Secret");

builder.Services.AddAuthentication(x =>
{
    // 🚀 Standard-Authentifizierungsmethode auf JWT setzen
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(x => {
        x.RequireHttpsMetadata = false; // ❌ HTTPS für Tokenvalidierung nicht erforderlich (nur für Entwicklung)
        x.SaveToken = true; // ✅ Das Token nach der Validierung speichern


        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true, // ✅ Prüfe, ob der Signaturschlüssel gültig ist
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key)), // 🔑 Der geheime Schlüssel für die Tokenprüfung

            ValidateIssuer = false, // ❌ Den "Issuer" (wer das Token ausgestellt hat) nicht prüfen
            ValidateAudience = false // ❌ Die "Audience" (wer das Token empfangen darf) nicht prüfen
        };
    }); ;

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options => {

    // ➤ JWT-Authentifizierung in Swagger aktivieren
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description =
            "JWT Authorization header using the Bearer scheme. \r\n\r\n " +
            "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
            "Example: \"Bearer 12345abcdef\"", // 📌 Anleitung für den Nutzer

        Name = "Authorization", // 📌 Name des Headers
        In = ParameterLocation.Header, // 📌 Das Token wird im Header der Anfrage übermittelt
        Scheme = "Bearer" //📌 Authentifizierungsschema "Bearer"
    });

    // ➤ Sicherheitsanforderung zu Swagger hinzufügen
    options.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme, // 📌 Bezieht sich auf die oben definierte Sicherheitsdefinition
                                Id = "Bearer" // 📌 Gleiche ID wie oben
                            },
                Scheme = "oauth2", // 📌 Wird als "OAuth2" markiert, aber nutzt JWT
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>() // 📌 Hier könnten spezifische Berechtigungen definiert werden, aber es bleibt leer
        }
    });

});


builder.Services.AddSwaggerGen();








var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
