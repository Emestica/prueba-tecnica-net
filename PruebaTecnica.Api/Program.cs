using PruebaTecnica.Api.Domain.ADO.Repository;
using PruebaTecnica.Api.Domain.ADO.Service;
using PruebaTecnica.Api.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Singletons
builder.Services.AddSingleton<IBaseDatosServicio, BaseDatosServicio>();

// General
builder.Services.AddScoped<ISecurityServices, SecurityServices>();
builder.Services.AddScoped<IConsultingRepository, ConsultingRepository>();
builder.Services.AddScoped<ITransactionalRepository, TransactionalRepository>();
builder.Services.AddScoped<ISecurityRepository, SecurityRepository>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
