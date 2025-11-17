using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using parcial_backend.Controllers;
using parcial_backend.Data;
using parcial_backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_Parcial
{
    public class PartidoControllerTest : TestBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PartidoController _controller;

        public PartidoControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            _context = new ApplicationDbContext(options);
            _controller = new PartidoController(_context);
            _context.Database.EnsureCreated();
            _context.Database.EnsureDeleted();
        }

        private Matches crearPartidoValido(int id = 0)
        {
            var partido = new Matches()
            {
                Id = id,
                Fecha = new DateTime(2023, 12, 01),
                EquipoLocalId = 1,
                EquipoVisitanteId = 2,
                GolesLocal = 3,
                GolesVisitante = 2
            };
            if (id > 0)
            {
                partido.Id = id;
            }
            return partido;
        }

        [Fact]
        public async Task TestCrearPartido()
        {
            // Arrange
            var partido = crearPartidoValido();
            // Act
            var result = await _controller.CrearPartido(partido);
            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            // Usar reflexión para acceder a la propiedad del objeto anónimo
            var messageProperty = okResult.Value.GetType().GetProperty("message");
            Assert.NotNull(messageProperty); // Verificar que la propiedad existe

            var messageValue = messageProperty.GetValue(okResult.Value) as string;
            Assert.NotNull(messageValue);
            Assert.Equal("Partido creado exitosamente", messageValue);
            //Verifica que el jugador se haya agregado a la base de datos
            var partidoInDb = await _context.Matches.FirstOrDefaultAsync();
            Assert.NotNull(partidoInDb);
            Assert.Equal(1, partidoInDb.EquipoLocalId);
            Assert.Equal(2, partidoInDb.EquipoVisitanteId);
        }

        [Fact]
        public async Task TestListaPartidos() 
        {
            var partidos = new List<Matches>()
            {
                crearPartidoValido(1),
                crearPartidoValido(2),
                crearPartidoValido(3)
            };

            await _context.Matches.AddRangeAsync(partidos);
            await _context.SaveChangesAsync();

            var result = await _controller.ListaPartido();

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnPartidos = Assert.IsType<List<Matches>>(okResult.Value);
            Assert.Equal(3, returnPartidos.Count);
        }

        [Fact]
        public async Task testVerPartidos() 
        {
            var partido = crearPartidoValido(1);
            await _context.Matches.AddAsync(partido);
            await _context.SaveChangesAsync();

            var result = await _controller.VerPartidos(1);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnPartido = Assert.IsType<Matches>(okResult.Value);
            Assert.Equal(1, returnPartido.Id);
            Assert.Equal(1, returnPartido.EquipoLocalId);
        }

        [Fact]
        public async Task TestEditarPartido() 
        {
            var partidoExistente = new Matches()
            {
                Id = 1,
                Fecha = new DateTime(2023, 12, 01),
                EquipoLocalId = 1,
                EquipoVisitanteId = 2,
                GolesLocal = 3,
                GolesVisitante = 2
            };

            await _context.Matches.AddAsync(partidoExistente);
            await _context.SaveChangesAsync();

            var partidoActualizado = new Matches()
            {
                Id = 1,
                Fecha = new DateTime(2023, 12, 05),
                EquipoLocalId = 1,
                EquipoVisitanteId = 2,
                GolesLocal = 4,
                GolesVisitante = 2
            };

            var result = await _controller.EditarPartido(1, partidoActualizado);

            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Matches>(okResult.Value);

            Assert.Equal(new DateTime(2023, 12, 05), returnValue.Fecha);
            Assert.Equal(4, returnValue.GolesLocal);
            Assert.Equal(2, returnValue.GolesVisitante);
            Assert.Equal(1, returnValue.EquipoLocalId);
            Assert.Equal(2, returnValue.EquipoVisitanteId);

            var partidoInDb = await _context.Matches.FindAsync(1);
            Assert.Equal(4, partidoInDb.GolesLocal);
            Assert.Equal(2, partidoInDb.GolesVisitante);
        }

        [Fact]
        public async Task TestEliminarPartido() 
        {
            var partito = crearPartidoValido(1);
            await _context.Matches.AddAsync(partito);
            await _context.SaveChangesAsync();

            var partidoAntes = await _context.Matches.FindAsync(1);
            Assert.NotNull(partidoAntes);

            var result = await _controller.EliminarPartido(1);

            Assert.IsType<OkResult>(result);

            var partidoDespues = await _context.Matches.FindAsync(1);
            Assert.Null(partidoDespues);
        }
    }
}
