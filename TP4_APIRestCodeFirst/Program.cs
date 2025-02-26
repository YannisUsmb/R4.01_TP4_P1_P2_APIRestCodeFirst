using TP4_APIRestCodeFirst.Models.EntityFramework;
using Microsoft.EntityFrameworkCore;
using static TP4_APIRestCodeFirst.Models.Repository.IDataRepository;
using TP4_APIRestCodeFirst.Models.DataManager;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDataRepository<Utilisateur>, UtilisateurManager>();

builder.Services.AddDbContext<FilmRatingsDBContext>(options =>
  options.UseNpgsql(builder.Configuration.GetConnectionString("FilmRatingsDBContext")));

// Add services to the container.

builder.Services.AddControllers();
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
