using Microsoft.EntityFrameworkCore;
using TubeTrackAPI.Models;

namespace TubeTrackerAPI.Data
{
    public class ApiDbContext : DbContext
    {

        protected readonly IConfiguration Configuration;

        public DbSet<Movie> Movies { get; set; }

        public ApiDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // Use conection string in appsettings.json
            options.UseSqlServer(Configuration.GetConnectionString("ApiDbConectionString"));
        }
    }
}
