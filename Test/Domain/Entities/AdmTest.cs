using Microsoft.VisualStudio.TestPlatform.CrossPlatEngine;
using MinimalApi.Domain.Entities;

namespace Test.Domain.Entities;

[TestClass]
public class AdmTest
{
    [TestMethod]
    public void TestGetSetProps()
    {
        var adm = new Adm();

        adm.Id = 1;
        adm.Email = "teste@teste.com";
        adm.Password = "654321";
        adm.Profile = "Adm";

        Assert.AreEqual(1, adm.Id);
        Assert.AreEqual("teste@teste.com", adm.Email);
        Assert.AreEqual("654321", adm.Password);
        Assert.AreEqual("Adm", adm.Profile);
    }
}