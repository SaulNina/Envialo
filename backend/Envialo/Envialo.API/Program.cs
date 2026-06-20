using System.Text;
using Envialo.Domain.Ports.IRepositories;
using Envialo.Domain.Ports.IServices;
using Envialo.Infrastructure;
using Envialo.Infrastructure.Adapters.Repositories;
using Envialo.Infrastructure.Adapters.Service;
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
builder.Services.AddScoped<Envialo.Application.UseCases.UserUseCases.Commands.RegisterUserCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.UserUseCases.Commands.LoginCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Commands.CreateShipmentCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Queries.GetPendingShipmentsQuery>();
builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Queries.GetShipmentByIdQuery>();
builder.Services.AddScoped<Envialo.Application.UseCases.FareOfferUseCases.Commands.CreateFareOfferCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.FareOfferUseCases.Commands.AcceptFareOfferCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.FareOfferUseCases.Queries.GetOffersByShipmentQuery>();
builder.Services.AddScoped<Envialo.Application.UseCases.RatingUseCases.Commands.CreateRatingCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.RatingUseCases.Queries.GetUserRatingsQuery>();

builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Commands.StartTripCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Commands.CompleteTripCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Queries.GetTripByIdQuery>();

builder.Services.AddScoped<Envialo.Application.UseCases.ShipmentUseCases.Commands.CancelShipmentCommand>();
builder.Services.AddScoped<Envialo.Application.UseCases.TripUseCases.Commands.CancelTripCommand>();

builder.Services.AddScoped<Envialo.Application.UseCases.UserUseCases.Queries.GetUserProfileQuery>();
builder.Services.AddScoped<Envialo.Application.UseCases.UserUseCases.Commands.UpdateUserProfileCommand>();

builder.Services.AddScoped<Envialo.Application.UseCases.UploadUseCases.Commands.UploadImageCommand>();
builder.Services.AddHttpClient<Envialo.Domain.Ports.IServices.IStorageService, Envialo.Infrastructure.Adapters.Service.SupabaseStorageService>();
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