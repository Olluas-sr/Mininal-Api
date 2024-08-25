using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interface;

public interface IAdmService 
{
    public Adm? Login(LoginDTO loginDTO);

    public Adm AddAdm(Adm adm);

    List<Adm> GetAll( int? page);

    public Adm? GetID ( int id);
}