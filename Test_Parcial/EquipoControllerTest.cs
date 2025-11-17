using parcial_backend.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using parcial_backend.Controllers;
using parcial_backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using FluentAssertions;

namespace Test_Parcial
{
    public class EquipoControllerTest : TestBase
    {
        private readonly ApplicationDbContext _context;
        private readonly EquipoController _controller;
        public EquipoControllerTest()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new ApplicationDbContext(options);
            _controller = new EquipoController(_context);
            _context.Database.EnsureCreated();
            _context.Database.EnsureDeleted();
        }

        private Teams crearEquipoValido(int id = 0)
        {
            var equipo = new Teams()
            {
                Id = id,
                Nombre = "Equipo Test",
                Ciudad = "Ciudad Test",
                Estadio = "Estadio Test",
                Fundacion = 1900
            };

            if (id > 0)
            {
                equipo.Id = id;
            }

            return equipo;
        }

        [Fact]
        public async Task TestCrearEquipos() 
        {
            // Arrange
            var equipo = crearEquipoValido();
            // Act
            var result = await _controller.CrearEquipo(equipo);
            // Assert
            result.Should().NotBeNull();
            // Assert Result es OkObjectResult y su StatusCode es 200
            var okResult = Assert.IsType<OkObjectResult>(result);
            
            var messageProperty = okResult.Value?.GetType().GetProperty("message");
            Assert.NotNull(messageProperty);
            // Verifica el mensaje de respuesta
            var messageValue = messageProperty.GetValue(okResult.Value) as string;
            Assert.NotNull(messageValue);
            Assert.Equal("Equipo creado exitosamente", messageValue);
            //Verifica que el equipo se haya agregado a la base de datos
            var equipoInDb = await _context.Teams.FirstOrDefaultAsync();
            Assert.NotNull(equipoInDb);
            Assert.Equal("Equipo Test", equipoInDb.Nombre);
            Assert.Equal("Ciudad Test", equipoInDb.Ciudad);
        }

        [Fact]
        public async Task TestListaEquipos() 
        {
            // Arrange
            var equipo = new List<Teams>()
            {
                crearEquipoValido(1),
                crearEquipoValido(2),
                crearEquipoValido(3)
            };
            
            await _context.Teams.AddRangeAsync(equipo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.ListaEquipo();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<Teams>>(okResult.Value);
            Assert.Equal(3, returnValue.Count);
        }

        [Fact]
        public async Task TestVerEquipos() 
        {
            // Arrange
            var equipo = crearEquipoValido(1);
            await _context.Teams.AddAsync(equipo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.VerEquipos(1);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Teams>(okResult.Value);
            Assert.Equal(1, returnValue.Id);
            Assert.Equal("Equipo Test", returnValue.Nombre);
        }

        [Fact]
        public async Task TestEditarEquipo() 
        {
            // Arrange
            var equipoExistente = new Teams
            {
                Id = 1,
                Nombre = "Equipo Test",
                Ciudad = "Ciudad Test",
                Estadio = "Estadio Test",
                Fundacion = 1900
            };

            await _context.Teams.AddAsync(equipoExistente);
            await _context.SaveChangesAsync();

            var equipoActualizado = new Teams
            {
                Id = 1,
                Nombre = "Equipo Actualizado",
                Ciudad = "Ciudad Actualizada",
                Estadio = "Estadio Actualizado",
                Fundacion = 1950
            };

            // Act
            var result = await _controller.EditarEquipo(1, equipoActualizado);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<Teams>(okResult.Value);

            Assert.Equal("Equipo Actualizado", returnValue.Nombre);
            Assert.Equal("Ciudad Actualizada", returnValue.Ciudad);

            // Verificar cambios en base ed datos
            var equipoInDb = await _context.Teams.FindAsync(1);
            Assert.Equal("Equipo Actualizado", equipoInDb.Nombre);
            Assert.Equal("Ciudad Actualizada", equipoInDb.Ciudad);
        }

        [Fact]
        public async Task TestEliminarEquipo() 
        {
            // Arrange
            var equipo = crearEquipoValido(1);
            await _context.Teams.AddAsync(equipo);
            await _context.SaveChangesAsync();

            // Verificar que el equipo existe antes de eliminar
            var equipoAntes = await _context.Teams.FindAsync(1);
            Assert.NotNull(equipoAntes);

            // Act
            var result = await _controller.EliminarEquipo(1);

            // Assert
            Assert.IsType<OkResult>(result);

            //Verificar que fue eliminado
            var equipoDespues = await _context.Teams.FindAsync(1);
            Assert.Null(equipoDespues);
        }
    }
}
