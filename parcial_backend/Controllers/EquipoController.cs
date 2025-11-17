using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using parcial_backend.Data;
using parcial_backend.Models;

namespace parcial_backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EquipoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContext Context { get; }

        public EquipoController(ApplicationDbContext context) { 
        
            _context = context;

        }

        [HttpPost]
        [Route("crearEquipo")]
        public async Task<IActionResult> CrearEquipo(Teams equipo)
        {
            await _context.Teams.AddAsync(equipo);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Equipo creado exitosamente" });
        }

        [HttpGet]
        [Route("ListaEquipos")]
        public async Task<ActionResult<IEnumerable<Teams>>> ListaEquipos()
        {
            var equipos = new List<Teams>();

            try
            {
                await using var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                // Usar una transacción para manejar el cursor
                await using var transaction = await connection.BeginTransactionAsync();

                await using var command = new NpgsqlCommand("CALL sp_getallteams()", connection, transaction);
                await command.ExecuteNonQueryAsync();

                await using var fetchCommand = new NpgsqlCommand("FETCH ALL IN mycursor", connection, transaction);
                await using var reader = await fetchCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    equipos.Add(new Teams
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Ciudad = reader.GetString(2),
                        Estadio = reader.GetString(3),
                        Fundacion = reader.GetInt32(4)
                    });
                }

                await reader.CloseAsync();
                await transaction.CommitAsync();

                return Ok(equipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener equipos: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("ListaEquipos2")]
        public async Task<ActionResult<IEnumerable<Teams>>> ListaEquipo()
        {
            try
            {
                var equipos = await _context.Teams.ToListAsync();
                return Ok(equipos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener equipos: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("verEquipos")]
        public async Task<IActionResult> VerEquipos(int id)
        {
            Teams equipo = await _context.Teams.FindAsync(id);
            if (equipo == null)
            {
                return NotFound();
            }
            return Ok(equipo);
        }

        [HttpGet("jugadoresEquipo/{equipoId}")]
        public async Task<IActionResult> GetJugadoresByEquipoId(int equipoId)
        {
            var jugadores = await _context.Players
                .Where(j => j.EquipoId == equipoId)
                .ToListAsync();

            if (jugadores == null || !jugadores.Any())
            {
                return NotFound(new { message = "No se encontraron jugadores para este equipo" });
            }

            return Ok(jugadores);
        }

        [HttpPut]
        [Route("EditarEquipo")]
        public async Task<IActionResult> EditarEquipo(int id, Teams equipo)
        {
            var equipoExistente = await _context.Teams.FindAsync(id);
            equipoExistente!.Nombre = equipo.Nombre; 
            equipoExistente.Ciudad = equipo.Ciudad;
            equipoExistente.Estadio = equipo.Estadio;
            equipoExistente.Fundacion = equipo.Fundacion;

            await _context.SaveChangesAsync();

            return Ok(equipoExistente);
        }

        [HttpDelete]
        [Route("EliminarEquipo")]
        public async Task<IActionResult> EliminarEquipo(int id)
        {
            var equipoBorrado = await _context.Teams.FindAsync(id);

            _context.Teams.Remove(equipoBorrado!);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
