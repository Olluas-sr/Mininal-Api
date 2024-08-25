
using MinimalApi.Domain.Enuns;

namespace MinimalApi.DTOs;
public class AdmDTO {
    public string Email { get; set; } = default!;

    public string Password { get; set; } = default!;
    public Profile? Profile { get; set; } = default!;

}