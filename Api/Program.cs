using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Enuns;
using MinimalApi.Domain.Interface;
using MinimalApi.Domain.ModelViews;
using MinimalApi.Domain.Services;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

#region Builder
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();
if(string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option => {
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer( option => {
    option.TokenValidationParameters = new TokenValidationParameters{
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddAuthorization();
builder.Services.AddScoped<IAdmService , AdmService>();
builder.Services.AddScoped<IVehicleService , VehicleService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen( option => 
{
    option.AddSecurityDefinition( "Bearer", new OpenApiSecurityScheme{
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira seu token JWT aqui"
    });

    option.AddSecurityRequirement(new OpenApiSecurityRequirement{
        {
            new OpenApiSecurityScheme{
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
} );

builder.Services.AddDbContext<DataBaseContext>(option => option.UseSqlServer(builder.Configuration.GetConnectionString("SqlServer")));
    
#endregion

#region App
var app = builder.Build();
#region Home
app.MapGet("/", () => Results.Json(new Home())).WithTags("Home");
#endregion

#region Adm

string generateJwtToken(Adm adm){
    if(string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials( securityKey, SecurityAlgorithms.HmacSha256 );

    var claims = new List<Claim>()
    {
        new Claim("Email", adm.Email),
        new Claim(ClaimTypes.Role, adm.Profile),
        new Claim("profile",adm.Profile)
    };

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/login", ([FromBody] LoginDTO loginDTO, IAdmService AdmService ) => {
    var adm = AdmService.Login(loginDTO);

    if(adm != null){
        string token = generateJwtToken(adm);
        return Results.Ok(new AdmLoged
        {
            Email = adm.Email,
            Profile = adm.Profile,
            Token = token
        });
    }
    else
        return Results.Unauthorized();
}).AllowAnonymous().WithTags("ADMs");

app.MapPost("/adm", ([FromBody]  AdmDTO admDTO, IAdmService admService ) => 
        {

        var validate = new ValidateErrors{
            Messages = new List<string>()
        };
        if(string.IsNullOrEmpty(admDTO.Email))
        {
            validate.Messages.Add("Email nao pode ser vazio !!!");
        }

                if(string.IsNullOrEmpty(admDTO.Password))
        {
            validate.Messages.Add("Senha nao pode ser vazia !!!");
        }

                if(admDTO.Profile == null)
        {
            validate.Messages.Add("Perfil nao pode ser vazio !!!");
        }

        var adm = new Adm{
            Email = admDTO.Email,
            Password = admDTO.Password,
            Profile = admDTO.Profile.ToString() ?? Profile.editor.ToString()
        };

        admService.AddAdm(adm);

        return Results.Created($"/adm/{adm.Id}", 
            new AdmModelView
            {
                Id = adm.Id,
                Email = adm.Email,
                Profile = adm.Profile
            });

        
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("ADMs");

app.MapGet("/adm", ([FromQuery] int? page, IAdmService admServiceice ) => 
    {
        var adms = new List<AdmModelView>();
        var admsList = admServiceice.GetAll(page);

        foreach(var adm in admsList)
        {
            adms.Add(new AdmModelView
            {
                Id = adm.Id,
                Email = adm.Email,
                Profile = adm.Profile
            }
            );
            
        }

        return Results.Ok(adms);
    }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("ADMs");

app.MapGet("/adm/{id}", ([FromQuery] int id, IAdmService admServiceice ) => 
    {
        var adm = admServiceice.GetID(id);
        if(adm == null) return Results.NotFound();
        return Results.Ok(new AdmModelView
            {
                Id = adm.Id,
                Email = adm.Email,
                Profile = adm.Profile
            });
    }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("ADMs");

#endregion

#region Veichle

ValidateErrors validateDTO ( VehicleDTO vehicleDTO )
{
        var validation = new ValidateErrors{
            Messages = new List<string>()
        };

        if(string.IsNullOrEmpty(vehicleDTO.Name))
            {
                validation.Messages.Add("O nome não pode ser vazio!!");
            }

            if(string.IsNullOrEmpty(vehicleDTO.Model))
            {
                validation.Messages.Add("O campo de modelo não pode ser vazio!!");
            }

            if(vehicleDTO.Year <= 1950)
            {
                validation.Messages.Add("O carro é muito antigo, nao aceitamos carros fabricados antes de 1951!");
            }

            return validation;
}

        app.MapPost("/vehicles", ([FromBody] VehicleDTO vehicleDTO, IVehicleService vehicleService ) => 
        {

            var validate = validateDTO(vehicleDTO);
                if(validate.Messages.Count > 0)
                {
                    return Results.BadRequest(validateDTO);
                }

            var vehicle = new Vehicle{
                Name = vehicleDTO.Name,
                Model = vehicleDTO.Model,
                Year = vehicleDTO.Year,
            };

            vehicleService.Add(vehicle);
            return Results.Created($"/vehicle/{vehicle.Id}", vehicle );
}).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

        app.MapGet("/vehicles", ([FromQuery] int? page, IVehicleService vehicleService ) => 
        {
            var vehicles = vehicleService.All(page);

            return Results.Ok(vehicles);
        }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Vehicles");

        app.MapGet("/vehicles/{id}", ([FromQuery] int id, IVehicleService vehicleService ) => 
        {
            var vehicle = vehicleService.IdFind(id);

            if(vehicle == null) return Results.NotFound();

            return Results.Ok(vehicle);
        }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" }).WithTags("Vehicles");

        app.MapPut("/vehicles/{id}", ([FromRoute] int id, VehicleDTO vehicleDTO,  IVehicleService vehicleService ) => 
        {

            var validate = validateDTO(vehicleDTO);
            if(validate.Messages.Count > 0)
            {
                return Results.BadRequest(validateDTO);
            }


            var vehicle = vehicleService.IdFind(id);

            if(vehicle == null) return Results.NotFound();

            vehicle.Name = vehicleDTO.Name;
            vehicle.Model = vehicleDTO.Model;
            vehicle.Year = vehicleDTO.Year;

            vehicleService.Update(vehicle);
            return Results.Ok(vehicle);
        }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

        app.MapDelete("/vehicles/{id}", ([FromRoute] int id, IVehicleService vehicleService ) => 
        {
            var vehicle = vehicleService.IdFind(id);

            if(vehicle == null) return Results.NotFound();

            vehicleService.Delete(vehicle);
            return Results.NoContent();
        }).RequireAuthorization().RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" }).WithTags("Vehicles");

#endregion

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();


app.Run();
    
#endregion
