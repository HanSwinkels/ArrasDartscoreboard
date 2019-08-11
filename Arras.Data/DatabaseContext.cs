using Arras.Data.Entity;
using Microsoft.EntityFrameworkCore;

namespace Arras.Data
{
    public class DatabaseContext : DbContext
    {
        /// <summary>
        /// A database set holding multiple <see cref="StandardMatchEntity"/>.
        /// </summary>
        public DbSet<StandardMatchEntity> StandardMatches { get; set; }

        /// <summary>
        /// A database set holding multiple <see cref="PlayerEntity"/>.
        /// </summary>
        public DbSet<PlayerEntity> Players { get; set; }

        /// <summary>
        /// A database set holding multiple <see cref="LegEntity"/>.
        /// </summary>
        public DbSet<LegEntity> Legs { get; set; }

        /// <inheritdoc />
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=arras.db");
        }
    }
}