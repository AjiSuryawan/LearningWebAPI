using Microsoft.EntityFrameworkCore;

namespace LearningWebAPI.Models
{
    public class MovieContext : DbContext
    {
        public MovieContext(DbContextOptions<MovieContext> options) : base (options) { 
            
        }

        public DbSet<Movie> Movies { get; set; } = null!;
        public DbSet<UserModel> Users { get; set; } = null!;

    }
}
