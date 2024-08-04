using MinimalApi.Domain.Entities;

namespace MinimalApi.Domain.Interface;

public interface IVehicleService 
{
    List<Vehicle> All(int? page = 1, string? name = null, string? model = null );

    Vehicle? IdFind ( int id );

    void Add ( Vehicle vehicle );

    void Update ( Vehicle vehicle );

    void Delete ( Vehicle vehicle );

}