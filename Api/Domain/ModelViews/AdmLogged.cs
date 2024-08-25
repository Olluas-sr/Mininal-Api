namespace MinimalApi.Domain.ModelViews;

public record AdmLoged 
{
    public required string Email { get; set; }
    public required string? Profile { get; set; }
    public required string Token { get; set;}
}