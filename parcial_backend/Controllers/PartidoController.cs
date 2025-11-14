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
    public class PartidoController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContext Context { get; }

        public PartidoController(ApplicationDbContext context)
        {

            _context = context;

        }

        [HttpPost]
        [Route("crearPartido")]
        public async Task<IActionResult> CrearPartido(Matches partido)
        {
            await _context.Matches.AddAsync(partido);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Partido creado exitosamente" });
        }

        [HttpGet]
        [Route("ListaPartidos")]
        public async Task<ActionResult<IEnumerable<Matches>>> ListaPartidos()
        {
            var partidos = new List<Matches>();

            try
            {
                await using var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                // Usar una transacción para manejar el cursor
                await using var transaction = await connection.BeginTransactionAsync();

                await using var command = new NpgsqlCommand("CALL sp_getallmatches()", connection, transaction);
                await command.ExecuteNonQueryAsync();

                await using var fetchCommand = new NpgsqlCommand("FETCH ALL IN mycursor", connection, transaction);
                await using var reader = await fetchCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    partidos.Add(new Matches
                    {
                        Id = reader.GetInt32(0),
                        Fecha = reader.GetDateTime(1),
                        GolesLocal = reader.GetInt32(2),
                        GolesVisitante = reader.GetInt32(3),
                        EquipoLocalId = reader.GetInt32(4),
                        EquipoVisitanteId = reader.GetInt32(5)
                    });
                }

                await reader.CloseAsync();
                await transaction.CommitAsync();

                return Ok(partidos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener equipos: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("verPartidos")]
        public async Task<IActionResult> VerPartidos(int id)
        {
            Matches partido = await _context.Matches.FindAsync(id);
            if (partido == null)
            {
                return NotFound();
            }
            return Ok(partido);
        }

        [HttpPut]
        [Route("EditarPartido")]
        public async Task<IActionResult> EditarPartido(int id, Matches partido)
        {
            var partidoExistente = await _context.Matches.FindAsync(id);
            partidoExistente!.Fecha = partido.Fecha;
            partidoExistente.EquipoLocal = partido.EquipoLocal;
            partidoExistente.EquipoVisitante = partido.EquipoVisitante;
            partidoExistente.GolesLocal = partido.GolesLocal;
            partidoExistente.GolesVisitante = partido.GolesVisitante;

            await _context.SaveChangesAsync();

            return Ok(partidoExistente);
        }

        [HttpDelete]
        [Route("EliminarPartido")]
        public async Task<IActionResult> EliminarPartido(int id)
        {
            var partidoBorrado = await _context.Matches.FindAsync(id);

            _context.Matches.Remove(partidoBorrado!);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
