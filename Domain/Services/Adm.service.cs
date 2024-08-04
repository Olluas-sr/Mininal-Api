using MinimalApi.Domain.Entities;
using MinimalApi.Domain.Interface;
using MinimalApi.DTOs;
using MinimalApi.Infrastructure.Db;

namespace MinimalApi.Domain.Services;

public class AdmService : IAdmService
{

    private readonly DataBaseContext _context;

    public AdmService(DataBaseContext context)
    {
        _context = context;
    }

    public Adm? Login(LoginDTO loginDTO)
    {
        var adm = _context.Adms.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }
}