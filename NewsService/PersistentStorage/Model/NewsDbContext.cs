using Microsoft.EntityFrameworkCore;

namespace PersistentStorage.Model
{
    public class NewsDbContext : DbContext
    {
        private readonly string _connectionString;

        public DbSet<NewsItemEntity> NewsItems { get; set; }

        public NewsDbContext()
        {
            _connectionString = "Data Source=./news.db";
        }

        public NewsDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<NewsItemEntity>()
                .HasIndex(nie => nie.Date);
        }
    }
}
