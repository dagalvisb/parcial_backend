using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace parcial_backend.Models
{
    public class Teams
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "La ciudad es obligatoria")]
        [MaxLength(50, ErrorMessage = "La ciudad no puede exceder 50 caracteres")]
        public string Ciudad { get; set; } = string.Empty;

        [Required(ErrorMessage = "El estadio es obligatorio")]
        [MaxLength(100, ErrorMessage = "El estadio no puede exceder 100 caracteres")]
        public string Estadio { get; set; } = string.Empty;

        [Required(ErrorMessage = "El año de fundación es obligatorio")]
        [Range(1800, 2024, ErrorMessage = "El año de fundación debe estar entre 1800 y 2024")]
        public int Fundacion { get; set; }

        //RELACIÓN: Un equipo tiene muchos jugadores
        [JsonIgnore]
        public ICollection<Players> Jugadores { get; set; } = new List<Players>();

        //RELACIÓN: Un equipo juega muchos partidos como local
        [JsonIgnore]
        public ICollection<Matches> PartidosLocal { get; set; } = new List<Matches>();

        //RELACIÓN: Un equipo juega muchos partidos como visitante
        [JsonIgnore]
        public ICollection<Matches> PartidosVisitante { get; set; } = new List<Matches>();
    }
}
