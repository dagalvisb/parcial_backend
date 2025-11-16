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
    public class JugadorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ApplicationDbContext Context { get; }

        public JugadorController(ApplicationDbContext context)
        {

            _context = context;

        }

        [HttpPost]
        [Route("crearJugador")]
        public async Task<IActionResult> CrearJugador(Players jugador)
        {
            await _context.Players.AddAsync(jugador);
            await _context.SaveChangesAsync();
            return Ok(new { message = "Jugador creado exitosamente" });
        }


        [HttpGet]
        [Route("ListaJugadores")]
        public async Task<ActionResult<IEnumerable<Players>>> ListaJugadores()
        {
            var jugadores = new List<Players>();

            try
            {
                await using var connection = (NpgsqlConnection)_context.Database.GetDbConnection();
                await connection.OpenAsync();

                // Usar una transacción para manejar el cursor
                await using var transaction = await connection.BeginTransactionAsync();

                await using var command = new NpgsqlCommand("CALL sp_getallplayers()", connection, transaction);
                await command.ExecuteNonQueryAsync();

                await using var fetchCommand = new NpgsqlCommand("FETCH ALL IN mycursor", connection, transaction);
                await using var reader = await fetchCommand.ExecuteReaderAsync();

                while (await reader.ReadAsync())
                {
                    jugadores.Add(new Players
                    {
                        Id = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Posicion = reader.GetString(2),
                        Edad = reader.GetInt32(3),
                        EquipoId = reader.GetInt32(4)
                    });
                }

                await reader.CloseAsync();
                await transaction.CommitAsync();

                return Ok(jugadores);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error al obtener equipos: {ex.Message}");
            }
        }

        [HttpGet]
        [Route("verJugadores")]
        public async Task<IActionResult> VerJugadores(int id)
        {
            Players jugador = await _context.Players.FindAsync(id);
            if (jugador == null)
            {
                return NotFound();
            }
            return Ok(jugador);
        }

        [HttpPut]
        [Route("EditarJugador")]
        public async Task<IActionResult> EditarJugador(int id, Players jugador)
        {
            var jugadorExistente = await _context.Players.FindAsync(id);
            jugadorExistente!.Nombre = jugador.Nombre;
            jugadorExistente.Posicion = jugador.Posicion;
            jugadorExistente.Edad = jugador.Edad;
            jugadorExistente.EquipoId = jugador.EquipoId;

            await _context.SaveChangesAsync();

            return Ok(jugadorExistente);
        }

        [HttpDelete]
        [Route("EliminarJugador")]
        public async Task<IActionResult> EliminarJugador(int id)
        {
            var jugadorBorrado = await _context.Players.FindAsync(id);

            _context.Players.Remove(jugadorBorrado!);

            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
