using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;
using MinimalApi.Domain.Entities;


namespace Test.Domain.Entities;

[TestClass]

public class vehicleTest
{
    [TestMethod]

    public void vehicleGetSetProps()
    {
        var vehicle = new Vehicle();

        vehicle.Id = 1;
        vehicle.Model = "Toyota";
        vehicle.Name = "Corola";
        vehicle.Year = 2020;

        Assert.AreEqual(1, vehicle.Id);
        Assert.AreEqual("Toyota", vehicle.Model);
        Assert.AreEqual("Corola", vehicle.Name);
        Assert.AreEqual(2020, vehicle.Year);
    }
}