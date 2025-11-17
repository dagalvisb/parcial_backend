using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using parcial_backend.Data;
using parcial_backend.Models;

namespace Test_Parcial
{
    public class TestBase : IDisposable
    {
        protected ApplicationDbContext context { get; private set; }

        protected ServiceProvider serviceProvider { get; private set; }

        public TestBase()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>(static options => {

                options.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
                options.EnableSensitiveDataLogging();
            });

            serviceProvider = services.BuildServiceProvider();
            context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            context.Database.EnsureCreated();
        }

        protected async Task SeedTestDatabse() { 
        
            var matches = new List<Matches>()
            {
                new Matches()
                {
                    Id=1,
                    Fecha= new DateTime(2023,10,10),
                    EquipoLocalId=1,
                    EquipoVisitanteId=2,
                    GolesLocal=2,
                    GolesVisitante=1
                },

                new Matches()
                {
                    Id=2,
                    Fecha= new DateTime(2023,11,15),
                    EquipoLocalId=3,
                    EquipoVisitanteId=4,
                    GolesLocal=0,
                    GolesVisitante=0
                }
            };

            var teams = new List<Teams>()
            {

                new Teams()
                {
                    Id=1,
                    Nombre="Equipo A",
                    Ciudad="Ciudad A",
                    Estadio="Estadio A",
                    Fundacion=1900
                },

                new Teams()
                {
                    Id=2,
                    Nombre="Equipo B",
                    Ciudad="Ciudad B",
                    Estadio="Estadio B",
                    Fundacion=1920
                },

            };

            var players = new List<Players>()
            {
                new Players()
                {
                    Id=1,
                    Nombre="Jugador 1",
                    Posicion="Delantero",
                    EquipoId=1
                },
                new Players()
                {
                    Id=2,
                    Nombre="Jugador 2",
                    Posicion="Defensa",
                    EquipoId=2
                }
            };

            context.Matches.AddRange(matches);
            context.Teams.AddRange(teams);
            context.Players.AddRange(players);
            await context.SaveChangesAsync();
        }

        protected void SetupValidModelState<T>(T model) where T : class
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);
            if (!isValid)
            {
                throw new InvalidOperationException("El modelo no es válido para las pruebas.");
            }
        }

        protected void SetupInvalidModelState<T>(T model, string expectedErrorMessage) where T : class
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(model, null, null);
            var validationResults = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
            bool isValid = System.ComponentModel.DataAnnotations.Validator.TryValidateObject(model, validationContext, validationResults, true);
            if (isValid)
            {
                throw new InvalidOperationException("El modelo es válido, se esperaba que fuera inválido para las pruebas.");
            }
            if (!validationResults.Any(vr => vr.ErrorMessage == expectedErrorMessage))
            {
                throw new InvalidOperationException($"El mensaje de error esperado '{expectedErrorMessage}' no se encontró en los resultados de validación.");
            }
        }
        public void Dispose()
        {
            // Código de limpieza común para las pruebas
            context?.Dispose();
            serviceProvider?.Dispose();
        }
    }
}
