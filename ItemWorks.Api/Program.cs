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

//Http Services
builder.Services.AddHttpClient<IUserService, UserService>(client =>
{
    var baseUrl = builder.Configuration["Services:UserService"];
    client.BaseAddress = new Uri(baseUrl);
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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
