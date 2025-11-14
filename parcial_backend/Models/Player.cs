using parcial_backend.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

public class Players
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
    public string Nombre { get; set; } = null!;

    [Required(ErrorMessage = "La posición es obligatoria")]
    [MaxLength(50, ErrorMessage = "La posición no puede exceder 50 caracteres")]
    public string Posicion { get; set; } = null!;

    [Range(1, 120, ErrorMessage = "La edad debe ser un número positivo entre 1 y 120")]
    public int Edad { get; set; }

    //CLAVE FORÁNEA
    public int EquipoId { get; set; }

    //RELACIÓN: Un jugador pertenece a un equipo
    [JsonIgnore]
    public Teams? Equipo { get; set; }
}
