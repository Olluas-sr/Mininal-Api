using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interface;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdmService , AdmService>();
builder.Services.AddScoped<IVehicleService , VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataBaseContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
    
#endregion

#region App
var app = builder.Build();
#region Home
    app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Adm
    app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdmService AdmService ) => {
    if(AdmService.Login(loginDTO) != null)
        return Results.Ok("Login efetuado com sucesso!");
    else
        return Results.Unauthorized();
}).WithTags("ADMs");
#endregion

#region Veichle
        app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService ) => 
        {
            var vehicle = new Vehicle{
                Name = vehicleDTO.Name,
                Model = vehicleDTO.Model,
                Year = vehicleDTO.Year,
            };

            vehicleService.Add(vehicle);
            return Results.Created($"/vehicle/{vehicle.Id}", vehicle );
}).WithTags("Vehicles");

        app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService ) => 
        {
            var vehicles = vehicleService.All(page);

            return Results.Ok(vehicles);
        }).WithTags("Vehicles");

        app.MapGet("/vehicles/{id}", ([FromQuery] int id, IVehicleService vehicleService ) => 
        {
            var vehicle = vehicleService.IdFind(id);

            if(vehicle == null) return Results.NotFound();

            return Results.Ok(vehicle);
        }).WithTags("Vehicles");

        app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO,  IVehicleService vehicleService ) => 
        {
            var vehicle = vehicleService.IdFind(id);

            if(vehicle == null) return Results.NotFound();

            vehicle.Name = vehicleDTO.Name;
            vehicle.Model = vehicleDTO.Model;
            vehicle.Year = vehicleDTO.Year;

            vehicleService.Update(vehicle);
            return Results.Ok(vehicle);
        }).WithTags("Vehicles");

        app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService ) => 
        {
            var vehicle = vehicleService.IdFind(id);

            if(vehicle == null) return Results.NotFound();

            vehicleService.Delete(vehicle);
            return Results.NoContent();
        }).WithTags("Vehicles");

#endregion

app.UseSwagger();
app.UseSwaggerUI();
app.Run();
    
#endregion
