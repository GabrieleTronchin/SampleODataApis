using Classic.Odata.DataModel.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Classic.Odata.DataModel
{
    public class WeatherContext : DbContext, IDesignTimeDbContextFactory<WeatherContext>
    {
        WeatherContext IDesignTimeDbContextFactory<WeatherContext>.CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<WeatherContext>();
            optionsBuilder.UseSqlServer("Data Source=(localdb)\\mssqllocaldb;Initial Catalog=MySampleDB;Integrated Security=True;MultipleActiveResultSets=True");
            return new WeatherContext(optionsBuilder.Options);
        }

        public WeatherContext()
        {
        }

        public WeatherContext(DbContextOptions options) : base(options)
        {
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<WeatherForecastModel>()
                     .HasKey(p => new { p.Id });


        }

        public DbSet<WeatherForecastModel> WeatherForecasts { get; set; }

    }
}