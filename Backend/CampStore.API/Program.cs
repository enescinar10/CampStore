using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using CampStore.BusinessLayer;
using CampStore.DataAccessLayer;

var builder = WebApplicationBuilder.CreateBuilder(args);

// 🔐 JWT Konfigürasyonu
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });

// 📋 CORS Konfigürasyonu
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// 🎯 Service Registration
builder.Services.AddScoped<PersonelBL>();
builder.Services.AddScoped<KategoriBL>();
builder.Services.AddScoped<UrunBL>();
builder.Services.AddScoped<MusteriBL>();
builder.Services.AddScoped<SatisBL>();
builder.Services.AddScoped<FaturaBL>();
builder.Services.AddScoped<StokBL>();
builder.Services.AddScoped<LogBL>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
