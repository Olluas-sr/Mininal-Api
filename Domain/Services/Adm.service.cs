using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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

    public Adm AddAdm (Adm adm)
    {
        _context.Adms.Add(adm);
        _context.SaveChanges();
        return adm;
    }

    public List<Adm> GetAll(int? page)
    {
        var query = _context.Adms.AsQueryable();

        int itemsPerPage = 10;
        if(page != null){
        query = query.Skip(((int)page - 1) * itemsPerPage).Take(itemsPerPage); 
        }
        return query.ToList();
    }

    public Adm? GetID(int id)
    {
        return _context.Adms.Where(a => a.Id == id).FirstOrDefault();
    }

    public Adm? Login(LoginDTO loginDTO)
    {
        var adm = _context.Adms.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        return adm;
    }
}