using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parcial_backend.Controllers;
using parcial_backend.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Parcial
{
    public class JugadorControllerTest : TestBase
    {
        private readonly ApplicationDbContext _context;
        private readonly JugadorController _controller;

        public JugadorControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new JugadorController(_context);
            _context.Database.EnsureCreated();
            _context.Database.EnsureDeleted();
        }

        private Players crearJugadorValido(int id = 0)
        {
            var jugador = new Players()
            {
                Id = id,
                Nombre = "Jugador Test",
                Posicion = "Delantero",
                Edad = 25,
                EquipoId = 1
            };
            if (id > 0)
            {
                jugador.Id = id;
            }
            return jugador;
        }

        [Fact]
        public async Task TestCrearJugador() 
        {
            // Arrange
            var jugador = crearJugadorValido();
            // Act
            var result = await _controller.CrearJugador(jugador);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Usar reflexión para acceder a la propiedad del objeto anónimo
            var messageProperty = okResult.Value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty); // Verificar que la propiedad existe
            
            var messageValue = messageProperty.GetValue(okResult.Value) as string;
            Assert.NotNull(messageValue);
            Assert.Equal("Jugador creado exitosamente", messageValue);
            //Verifica que el jugador se haya agregado a la base de datos
            var jugadorInDb = await _context.Players.FirstOrDefaultAsync();
            Assert.NotNull(jugadorInDb);
            Assert.Equal("Jugador Test", jugadorInDb.Nombre);
            Assert.Equal("Delantero", jugadorInDb.Posicion);

        }

        [Fact]
        public async Task TestListaJugador() 
        {
            var jugadores = new List<Players>()
            {
                crearJugadorValido(1),
                crearJugadorValido(2),
                crearJugadorValido(3)
            };

            await _context.Players.AddRangeAsync(jugadores);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ListaJugador();
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnPlayers = Assert.IsType<List<Players>>(okResult.Value);
            Assert.Equal(3, returnPlayers.Count);
        }

        [Fact]
        public async Task TestVerJugador() 
        {
            var jugador = crearJugadorValido(1);
            await _context.Players.AddAsync(jugador);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.VerJugadores(1);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnPlayer = Assert.IsType<Players>(okResult.Value);
            Assert.Equal(1, returnPlayer.Id);
            Assert.Equal("Jugador Test", returnPlayer.Nombre);
        }

        [Fact]
        public async Task TestEditarJugador() 
        {
            var jugadoeExistente = new Players()
            {
                Id = 1,
                Nombre = "Jugador Existente",
                Posicion = "Mediocampista",
                Edad = 28,
                EquipoId = 1
            };

            await _context.Players.AddAsync(jugadoeExistente);
            await _context.SaveChangesAsync();

            var jugadorActualizado = new Players()
            {
                Id = 1,
                Nombre = "Jugador Actualizado",
                Posicion = "Defensor",
                Edad = 30,
                EquipoId = 2
            };

            // Act
            var result = await _controller.EditarJugador(1, jugadorActualizado);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Players>(okResult.Value);
            Assert.Equal("Jugador Actualizado", returnValue.Nombre);
            Assert.Equal("Defensor", returnValue.Posicion);
            Assert.Equal(30, returnValue.Edad);
            Assert.Equal(2, returnValue.EquipoId);

            // Verificar que los cambios se hayan guardado en la base de datos
            var jugadorInDb = await _context.Players.FindAsync(1);
            Assert.NotNull(jugadorInDb);
            Assert.Equal("Jugador Actualizado", jugadorInDb.Nombre);
            Assert.Equal("Defensor", jugadorInDb.Posicion);
        }

        [Fact]
        public async Task TestEliminarJugador() 
        {
            var jugador = crearJugadorValido(1);
            await _context.Players.AddAsync(jugador);
            await _context.SaveChangesAsync();

            // Verificar que existe antes de eliminar   
            var jugadorAntes = await _context.Players.FindAsync(1);
            Assert.NotNull(jugadorAntes);

            // Act
            var result = await _controller.EliminarJugador(1);
            // Assert
            Assert.IsType<OkResult>(result);
            // Verificar que se haya eliminado de la base de datos
            var jugadorDespues = await _context.Players.FindAsync(1);
            Assert.Null(jugadorDespues);
        }
    }
}
