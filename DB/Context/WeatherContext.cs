using DB.Entities;
using Microsoft.EntityFrameworkCore;

namespace DB.Context
{
    // La clase WeatherContext hereda de DbContext, que es proporcionada por Entity Framework Core.
    public class WeatherContext : DbContext
    {
        // Constructor que toma opciones de DbContext (por ejemplo, cadena de conexión).
        public WeatherContext(DbContextOptions<WeatherContext> options) : base(options)
        {
            // No es necesario implementar nada específico en el constructor en este caso.
        }

        // DbSet<T>
        // DbSet para las entidades City y WeatherRecord.
        public DbSet<City> Cities { get; set; }
        public DbSet<WeatherRecord> WeatherRecords { get; set; }

        // Configurar el modelo de base de datos.
        // Se especifica el nombre de las tablas para las entidades City y WeatherRecord.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<City>().ToTable("city");
            modelBuilder.Entity<WeatherRecord>().ToTable("weather_record");
        }
    }
}
