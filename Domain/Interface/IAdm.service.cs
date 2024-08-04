using MinimalApi.Domain.Entities;
using MinimalApi.DTOs;

namespace MinimalApi.Domain.Interface;

public interface IAdmService 
{
    public Adm? Login(LoginDTO loginDTO);

}