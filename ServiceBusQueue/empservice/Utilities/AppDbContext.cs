using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer;
using empservice.Models;
using Microsoft.Extensions.Configuration;
using System; 

namespace empservice.Utilities
{
    /// <summary>
    /// Represents the application's database context for Employee data.
    /// </summary>
    public class AppDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        public DbSet<Employee> Employees { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="AppDbContext"/> class.
        /// </summary>
        /// <param name="configuration">The configuration instance to get the connection string.</param>
        public AppDbContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Configures the database connection and options.
        /// </summary>
        /// <param name="optionsBuilder">The options builder for configuring the database connection.</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }

        /// <summary>
        /// Configures the entity mappings and relationships.
        /// </summary>
        /// <param name="modelBuilder">The model builder for configuring entities.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired();
                entity.Property(e => e.Email).IsRequired();
                entity.Property(e => e.Department).IsRequired();
                entity.Property(e => e.HireDate).IsRequired();
                entity.Property(e => e.Salary).HasColumnType("decimal(18,2)");
                entity.Property(e => e.PhoneNumber).IsRequired();
            });
        }

        /// <summary>
        /// Retrieves all Employee records from the database.
        /// </summary>
        /// <returns>A list of all Employee records.</returns>
        public async Task<List<Employee>> GetAllEmployees()
        {
            return await Employees.ToListAsync();
        }
    }

}
