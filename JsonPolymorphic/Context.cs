using Microsoft.EntityFrameworkCore;

namespace JsonPolymorphic
{
    public class MyDbContext() : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseInMemoryDatabase("TestDB");
        }

        public DbSet<Item> Items { get; set; }

        /// <summary>
        /// https://learn.microsoft.com/en-us/ef/core/modeling/
        /// https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/?tabs=vs
        ///
        /// Add-Migration MigrationName
        /// Update-Database
        /// Update-Database MigrationName
        /// Remove-Migration
        /// Bundle-Migration
        /// Script-Migration MigrationName
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ItemConfigurations).Assembly); //get all configurations from the assembly

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(string)))
            {
                if (!property.GetMaxLength().HasValue)
                    property.SetMaxLength(50);
            }

            foreach (var property in modelBuilder.Model.GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(DateTimeOffset)))
            {
                if (!property.GetPrecision().HasValue)
                    property.SetPrecision(4);
            }
        }
    }
}