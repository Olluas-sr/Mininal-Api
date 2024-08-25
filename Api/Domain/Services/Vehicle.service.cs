using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interface;
using MinimalApi.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace MinimalApi.Domain.Services;

public class VehicleService : IVehicleService
{

        private readonly DataBaseContext _context;

        public VehicleService(DataBaseContext context)
    {
        _context = context;
    }

    public List<Vehicle> All(int? page = 1, string? name = null, string? model = null)
    {
        var query = _context.Vehicles.AsQueryable();
        if(!string.IsNullOrEmpty(name))
        {
            query = query.Where( v => EF.Functions.Like(v.Name.ToLower(), $"%{name.ToLower()}%"));
        }

        int itemsPerPage = 10;
        if(page != null){
        query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage); 
        }
        return query.ToList();


    }

    public void Add(Vehicle vehicle)
    {
        _context.Vehicles.Add(vehicle);
        _context.SaveChanges();
    }
    public void Delete(Vehicle vehicle)
    {
        _context.Vehicles.Remove(vehicle);
        _context.SaveChanges();
    }

    public Vehicle? IdFind(int id)
    {
        return _context.Vehicles.Where(v => v.Id == id).FirstOrDefault();
    }

    public void Update(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        _context.SaveChanges();
    }
}