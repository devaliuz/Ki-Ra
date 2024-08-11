using Microsoft.EntityFrameworkCore;

namespace KiRa.Infrastructure.Models.DB_Model
{
    public class CommandDbContext : DbContext
    {
        private readonly string _connectionString;

        public CommandDbContext(string connectionString)
        {
            _connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(_connectionString);
        }

        public DbSet<CommandEntry> Commands { get; set; }
        public DbSet<AnswerEntry> Answers { get; set; }
        public DbSet<SynonymEntry> Synonyms { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommandEntry>()
                .HasMany(c => c.Answers)
                .WithOne(a => a.Command)
                .HasForeignKey(a => a.CommandId);

            modelBuilder.Entity<CommandEntry>()
                .HasMany(c => c.Synonyms)
                .WithOne(s => s.Command)
                .HasForeignKey(s => s.CommandId);
        }
    }
}