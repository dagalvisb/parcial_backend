using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using parcial_backend.Data;

namespace Test_Parcial
{
    internal class TestBase : IDisposable
    {
        protected ApplicationDbContext context { get; private set; }

        protected ServiceProvider serviceProvider { get; private set; }

        public TestBase()
        {
            var services = new ServiceCollection();

            services.AddDbContext<ApplicationDbContext>()
        }
        public void Dispose()
        {
            // Código de limpieza común para las pruebas
            context?.Dispose();
            serviceProvider?.Dispose();
        }
    }
}
