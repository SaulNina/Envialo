using System.Text;
using Envialo.Application.Abstractions;
using Envialo.Application.Ports;
using Envialo.Infrastructure.Persistence;
using Envialo.Infrastructure.Persistence.Repositories;
using Envialo.Infrastructure.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurar Base de Datos (Supabase / PostgreSQL)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// 2. Inyección de Dependencias (Registrar Puertos y Adaptadores)
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IShipmentRepository, ShipmentRepository>();
builder.Services.AddScoped<IFareOfferRepository, FareOfferRepository>();
builder.Services.AddScoped<ITripRepository, TripRepository>();
builder.Services.AddScoped<IRatingRepository, RatingRepository>();

builder.Services.AddSingleton<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// 3. Registrar Casos de Uso (Puedes usar una librería como Scrutor o inyectarlos manualmente así)
builder.Services.AddScoped<Envialo.Application.UseCases.UserUseCases.Commands.RegisterUserUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.UserUseCases.Commands.LoginUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Commands.CreateShipmentUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Queries.GetPendingShipmentsUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Queries.GetShipmentByIdUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.FareOfferUseCases.Commands.CreateFareOfferUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.FareOfferUseCases.Commands.AcceptFareOfferUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.FareOfferUseCases.Queries.GetOffersByShipmentUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.RatingUseCases.Commands.CreateRatingUseCase>();

builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Commands.StartTripUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Commands.CompleteTripUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Queries.GetTripByIdUseCase>();

builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Commands.CancelShipmentUseCase>();
builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Commands.CancelTripUseCase>();
// (Agrega aquí el resto de tus UseCases conforme vayas creando tus controladores)

// 4. Configurar Autenticación JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
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
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
        };
    });

// 5. Configurar Controladores y Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Ingresa tu token JWT aquí (NO es necesario escribir 'Bearer ' antes)."
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// 6. Configurar el Pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();