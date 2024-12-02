using FluentValidation.AspNetCore;
using FluentValidation;
using ItemWorks.Api.Application.Core.Services.User;
using ItemWorks.Api.Domain.Repositories;
using ItemWorks.Api.Domain.Repositories.Base;
using ItemWorks.Api.Infrastructure.Data;
using ItemWorks.Api.Infrastructure.RepositorieImpl;
using ItemWorks.Api.Infrastructure.RepositorieImpl.Base;
using Microsoft.EntityFrameworkCore;
using ItemWorks.Api.Application.Core.Application.ItemWorks.Command;
using ItemWorks.Api.Application.Core.Mapping;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Database connection
var connectionString = builder.Configuration.GetConnectionString("SqlConnection");
builder.Services.AddDbContext<ItemWorkDbContext>(x => x.UseSqlServer(connectionString));

// Load validators from api project
builder.Services.AddFluentValidationAutoValidation().AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssembly(typeof(ItemWorkCreateCmd).Assembly);


// Adding MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly((typeof(ItemWorkCreateCmd).Assembly)));

//AutoMapper
builder.Services.AddAutoMapper(typeof(ItemWorkMapping).Assembly);

// Adding repository
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IItemWorkRepository, ItemWorkRespository>();

//Adding JWT configuration
builder.Services.AddAuthentication(config =>
{
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config =>
{
    config.RequireHttpsMetadata = false;
    config.SaveToken = true;
    config.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
        ValidateIssuerSigningKey = true,
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero,
    };
});

builder.Services.AddAuthorization();

//Http Services
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    var baseUrl = builder.Configuration["Services:UserService"];
    client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("NewPolicy", app =>
    {
        app.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("NewPolicy");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
