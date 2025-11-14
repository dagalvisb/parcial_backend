using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using parcial_backend.Models;

namespace parcial_backend.Models
{
    public class Matches
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }

        //CLAVE FORÁNEA Equipo Local
        public int EquipoLocalId { get; set; }

        //CLAVE FORÁNEA Equipo Visitante  
        public int EquipoVisitanteId { get; set; }

        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }

        //RELACIÓN: Un partido tiene un equipo local
        [JsonIgnore]
        public Teams? EquipoLocal { get; set; }

        //RELACIÓN: Un partido tiene un equipo visitante
        [JsonIgnore]
        public Teams? EquipoVisitante { get; set; }
    }
}