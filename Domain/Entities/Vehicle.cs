using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalApi.Domain.Entities;

public class Vehicle {

    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [StringLength(255)]
    public string Name { get; set; } = default!;

    [Required]
    [StringLength(50)]
    public string Model { get; set; } = default!;

    [Required]
    [StringLength(12)]
    public int Year { get; set; } = default!;
}