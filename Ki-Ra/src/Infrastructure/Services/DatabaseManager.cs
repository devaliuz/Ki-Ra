using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;

namespace KiRa.Infrastructure.Services
{
    public class DatabaseManager
    {
        private readonly string _connectionString;

        public DatabaseManager(string dbPath)
        {
            _connectionString = $"Data Source={dbPath}";
            EnsureDatabaseCreated();
        }

        private void EnsureDatabaseCreated()
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                if (context.Database.EnsureCreated())
                {
                    InitializeBasicCommands();
                }
            }
        }

        private void InitializeBasicCommands()
        {
            // Hallo
            AddCommand("hallo", "Guten Tag! Was kann ich für Sie tun?");
            AddCommand("hallo", "Hi! Wie kann ich Ihnen behilflich sein?");
            AddCommand("hallo", "Grüß Gott! Freut mich, Sie zu hören.");
            AddCommand("hallo", "Servus! Was führt Sie zu mir?");
            AddCommand("hallo", "Moin! Schön, dass Sie da sind.");
            AddCommand("hallo", "Hallo! Wie kann ich Ihnen helfen?");
            AddCommand("hallo", "Cheers du kleiner Sack.");
            // Wie geht es dir?
            AddCommand("wie geht es dir", "Mir geht es hervorragend, vielen Dank!");
            AddCommand("wie geht es dir", "Ich bin bestens gelaunt, und selbst?");
            AddCommand("wie geht es dir", "Ich fühle mich großartig, danke der Nachfrage.");
            AddCommand("wie geht es dir", "Alles bestens bei mir, wie sieht's bei Ihnen aus?");
            AddCommand("wie geht es dir", "Ich bin in Topform, und Sie?");
            AddCommand("wie geht es dir", "Mir geht es gut, danke der Nachfrage!");
            AddCommand("wie geht es dir", "Ausgezeichnet, und Ihnen?");

            AddCommand("was kannst du", "Ich kann Sprache erkennen und auf verschiedene Befehle reagieren.");
            //Witz
            AddCommand("witz", "Was ist orange und klingt wie ein Papagei? Eine Karotte!");
            AddCommand("witz", "Warum können Geister nicht lügen? Weil man sie durchschaut.");
            AddCommand("witz", "Was sagt ein Pirat am Computer? Shiver me timbers!");
            AddCommand("witz", "Was ist das Lieblingsgetränk eines Baumes? Wurzelbier.");
            AddCommand("witz", "Was ist grün und hüpft durch den Wald? Ein Frosch im Schlafanzug.");
            AddCommand("witz", "Warum tragen Bienen Helme? Weil sie sich in Luft auflösen.");
            AddCommand("witz", "Was ist schwarz-weiß und sitzt auf einem Baum? Ein hungriger Panda.");
            AddCommand("witz", "Was sagt ein Mathematiker zu seinem Freund? Ich bin so glücklich, dass ich Pi mal Daumen sagen kann.");
            AddCommand("witz", "Warum können Fische nicht Basketball spielen? Weil sie Angst vor dem Netz haben.");
            AddCommand("witz", "Was ist das Gegenteil von einem Hotdog? Ein kalter Kater.");
            AddCommand("witz", "Was ist das Lieblingsessen eines Astronauten? Ufos.");
            AddCommand("witz", "Was ist der Unterschied zwischen einem Klavier und einem Fisch? Man kann ein Klavier stimmen, aber man kann einen Fisch nicht klavieren.");
            AddCommand("witz", "Warum tragen Giraffen keine Socken? Weil sie lange Hälse haben.");
            AddCommand("witz", "Was ist das Lieblingsessen eines Schneemanns? Eiszapfen.");
            AddCommand("witz", "Warum gehen Ameisen nicht in die Kirche? Weil sie Insekten sind.");
            AddCommand("witz", "Was ist das Lieblingsessen eines Computers? Chips.");
            AddCommand("witz", "Was ist das Lieblingsgetränk eines Programmierers? Java.");
            AddCommand("witz", "Warum sind Tomaten so rot? Weil sie sich schämen, dass sie nicht schwimmen können.");
            AddCommand("witz", "Was ist das Lieblingsessen eines Geistes? Buh-Spaghetti.");
            AddCommand("witz", "Warum tragen Vögel keine Schuhe? Weil sie schon Federn haben.");

            // Auf Wiedersehen
            AddCommand("auf wiedersehen", "Machen Sie es gut! Bis bald.");
            AddCommand("auf wiedersehen", "Passen Sie auf sich auf! Wir sehen uns.");
            AddCommand("auf wiedersehen", "Alles Gute! Hoffentlich bis bald.");
            AddCommand("auf wiedersehen", "Einen schönen Tag noch! Bis zum nächsten Mal.");
            AddCommand("auf wiedersehen", "Bleiben Sie gesund! Auf ein baldiges Wiedersehen.");
            AddCommand("auf wiedersehen", "Auf Wiedersehen! Ich hoffe, wir sprechen bald wieder.");
            AddCommand("auf wiedersehen", "Tschüss! Bis zum nächsten Mal.");
        }

        public void AddCommand(string command, string answer)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var existingCommand = context.Commands.FirstOrDefault(c => c.Command.ToLower() == command.ToLower());
                if (existingCommand == null)
                {
                    existingCommand = new CommandEntry { Command = command };
                    context.Commands.Add(existingCommand);
                    context.SaveChanges();
                }

                var answerEntry = new AnswerEntry { Answer = answer, CommandId = existingCommand.Id };
                context.Answers.Add(answerEntry);
                context.SaveChanges();
            }
        }

        public List<string> GetCommands()
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                return context.Commands.Select(c => c.Command).ToList();
            }
        }

        public List<string> GetRegularCommands()
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                return context.Commands
                    .Where(c => c.Command.ToLower() != "neuer befehl" && c.Command.ToLower() != "was kannst du")
                    .Select(c => c.Command)
                    .ToList();
            }
        }

        public List<string> GetAnswers(string command)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                return context.Commands
                    .Where(c => c.Command.ToLower() == command.ToLower())
                    .SelectMany(c => c.Answers.Select(a => a.Answer))
                    .ToList();
            }
        }

        public bool CommandExists(string command)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                return context.Commands.Any(c => c.Command.ToLower() == command.ToLower());
            }
        }

        public void AddNewCommand(string command, string answer)
        {
            AddCommand(command, answer);
        }

        public void DeleteCommand(string command)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var commandToDelete = context.Commands
                    .Include(c => c.Answers)
                    .FirstOrDefault(c => c.Command.ToLower() == command.ToLower());

                if (commandToDelete != null)
                {
                    context.Answers.RemoveRange(commandToDelete.Answers);
                    context.Commands.Remove(commandToDelete);
                    context.SaveChanges();
                }
            }
        }

        public void UpdateCommand(string oldCommand, string newCommand)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var commandToUpdate = context.Commands
                    .FirstOrDefault(c => c.Command.ToLower() == oldCommand.ToLower());

                if (commandToUpdate != null)
                {
                    commandToUpdate.Command = newCommand;
                    context.SaveChanges();
                }
            }
        }

        public void DeleteAnswer(string command, string answer)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var answerToDelete = context.Answers
                    .FirstOrDefault(a => a.Command.Command.ToLower() == command.ToLower() && a.Answer == answer);

                if (answerToDelete != null)
                {
                    context.Answers.Remove(answerToDelete);
                    context.SaveChanges();
                }
            }
        }
    }

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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommandEntry>()
                .HasMany(c => c.Answers)
                .WithOne(a => a.Command)
                .HasForeignKey(a => a.CommandId);
        }
    }

    public class CommandEntry
    {
        public int Id { get; set; }
        public string Command { get; set; }
        public List<AnswerEntry> Answers { get; set; } = new List<AnswerEntry>();
    }

    public class AnswerEntry
    {
        public int Id { get; set; }
        public string Answer { get; set; }
        public int CommandId { get; set; }
        public CommandEntry Command { get; set; }
    }
}