using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using KiRa.Infrastructure.Models.DB_Model;

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
                    InitializeSpecialCommands();
                }
            }
        }

        private void InitializeBasicCommands()
        {
            // Hallo
            AddCommandWithSynonymsAndAnswers("hallo", new List<string>
            {
                "hallo", "hi", "hey", "grüß gott", "guten tag", "servus", "moin", "grüezi", "sali"
            }, new List<string>
            {
                "Guten Tag! Was kann ich für Sie tun?",
                "Hi! Wie kann ich Ihnen behilflich sein?",
                "Grüß Gott! Freut mich, Sie zu hören.",
                "Servus! Was führt Sie zu mir?",
                "Moin! Schön, dass Sie da sind.",
                "Hallo! Wie kann ich Ihnen helfen?",
                "Cheers du kleiner Sack."
            });

            // Wie geht es dir?
            AddCommandWithSynonymsAndAnswers("wie geht es dir", new List<string>
            {
                "wie geht es dir", "wie geht's", "alles klar", "wie läuft's", "wie fühlst du dich",
                "wie ist deine stimmung", "bist du gut drauf", "alles gut bei dir"
            }, new List<string>
            {
                "Mir geht es hervorragend, vielen Dank!",
                "Ich bin bestens gelaunt, und selbst?",
                "Ich fühle mich großartig, danke der Nachfrage.",
                "Alles bestens bei mir, wie sieht's bei Ihnen aus?",
                "Ich bin in Topform, und Sie?",
                "Mir geht es gut, danke der Nachfrage!",
                "Ausgezeichnet, und Ihnen?"
            });

            //Witz
            AddCommandWithSynonymsAndAnswers("erzähle einen witz", new List<string>
            {
                "erzähle einen witz", "erzähl einen witz", "witz", "witze", "scherz", "joke",
                "etwas lustiges", "zum lachen", "sei lustig", "mach einen witz",
                "bringen sie mich zum lachen", "sei witzig", "erzähle etwas komisches"
            }, new List<string>
            {
                "Was ist orange und klingt wie ein Papagei? Eine Karotte!",
                "Warum können Geister nicht lügen? Weil man sie durchschaut.",
                "Was sagt ein Pirat am Computer? Shiver me timbers!",
                "Was ist das Lieblingsgetränk eines Baumes? Wurzelbier.",
                "Was ist grün und hüpft durch den Wald? Ein Frosch im Schlafanzug.",
                "Warum tragen Bienen Helme? Weil sie sich in Luft auflösen.",
                "Was ist schwarz-weiß und sitzt auf einem Baum? Ein hungriger Panda.",
                "Was sagt ein Mathematiker zu seinem Freund? Ich bin so glücklich, dass ich Pi mal Daumen sagen kann.",
                "Warum können Fische nicht Basketball spielen? Weil sie Angst vor dem Netz haben.",
                "Was ist das Gegenteil von einem Hotdog? Ein kalter Kater.",
                "Was ist das Lieblingsessen eines Astronauten? Ufos.",
                "Was ist der Unterschied zwischen einem Klavier und einem Fisch? Man kann ein Klavier stimmen, aber man kann einen Fisch nicht klavieren.",
                "Warum tragen Giraffen keine Socken? Weil sie lange Hälse haben.",
                "Was ist das Lieblingsessen eines Schneemanns? Eiszapfen.",
                "Warum gehen Ameisen nicht in die Kirche? Weil sie Insekten sind.",
                "Was ist das Lieblingsessen eines Computers? Chips.",
                "Was ist das Lieblingsgetränk eines Programmierers? Java.",
                "Warum sind Tomaten so rot? Weil sie sich schämen, dass sie nicht schwimmen können.",
                "Was ist das Lieblingsessen eines Geistes? Buh-Spaghetti.",
                "Warum tragen Vögel keine Schuhe? Weil sie schon Federn haben."
            });

            // Auf Wiedersehen
            AddCommandWithSynonymsAndAnswers("auf wiedersehen", new List<string>
            {
                "auf wiedersehen", "tschüss", "bis bald", "ciao", "adieu", "mach's gut", "bis dann",
                "leb wohl", "auf wiederhören", "bis zum nächsten mal", "schönen tag noch"
            }, new List<string>
            {
                "Machen Sie es gut! Bis bald.",
                "Passen Sie auf sich auf! Wir sehen uns.",
                "Alles Gute! Hoffentlich bis bald.",
                "Einen schönen Tag noch! Bis zum nächsten Mal.",
                "Bleiben Sie gesund! Auf ein baldiges Wiedersehen.",
                "Auf Wiedersehen! Ich hoffe, wir sprechen bald wieder.",
                "Tschüss! Bis zum nächsten Mal."
            });
        }

        private void InitializeSpecialCommands()
        {
            // Befehle verwalten
            AddCommandWithSynonyms("befehle verwalten", new List<string>
            {
                "befehle verwalten", "befehl hinzufügen", "befehl löschen", "befehl ändern",
                "kommandos verwalten", "kommando hinzufügen", "kommando löschen", "kommando ändern",
                "verwaltung"
            });

            // Was kannst du
            AddCommandWithSynonyms("was kannst du", new List<string>
            {
                "was kannst du", "hilfe", "funktionen", "fähigkeiten", "was sind deine funktionen",
                "zeige mir deine fähigkeiten", "was machst du", "wie kannst du mir helfen"
            });
        }


        private void AddCommandWithSynonyms(string command, List<string> synonyms)
        {
            if (!CommandExists(command))
            {
                using (var context = new CommandDbContext(_connectionString))
                {
                    var newCommand = new CommandEntry { Command = command };
                    context.Commands.Add(newCommand);
                    context.SaveChanges();

                    foreach (var synonym in synonyms)
                    {
                        if (!context.Synonyms.Any(s => s.CommandId == newCommand.Id && s.Synonym.ToLower() == synonym.ToLower()))
                        {
                            context.Synonyms.Add(new SynonymEntry { CommandId = newCommand.Id, Synonym = synonym });
                        }
                    }
                    context.SaveChanges();
                }
            }
            else
            {
                foreach (var synonym in synonyms)
                {
                    AddSynonym(command, synonym);
                }
            }
        }

        private void AddCommandWithSynonymsAndAnswers(string command, List<string> synonyms, List<string> answers)
        {
            AddCommandWithSynonyms(command, synonyms);

            using (var context = new CommandDbContext(_connectionString))
            {
                var existingCommand = context.Commands.FirstOrDefault(c => c.Command.ToLower() == command.ToLower());
                if (existingCommand != null)
                {
                    foreach (var answer in answers)
                    {
                        if (!context.Answers.Any(a => a.CommandId == existingCommand.Id && a.Answer == answer))
                        {
                            context.Answers.Add(new AnswerEntry { CommandId = existingCommand.Id, Answer = answer });
                        }
                    }
                    context.SaveChanges();
                }
            }
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

                    // Füge den Befehlsnamen als Synonym hinzu
                    AddSynonym(command, command);
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
                    .Where(c => c.Command.ToLower() != "befehle verwalten" && c.Command.ToLower() != "was kannst du")
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
            if (!CommandExists(command))
            {
                AddCommand(command, answer);
                AddSynonym(command, command);  // Füge den Befehlsnamen als erstes Synonym hinzu
            }
            else
            {
                AddCommand(command, answer);
            }
        }

        public void DeleteCommand(string command)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var commandToDelete = context.Commands
                    .Include(c => c.Answers)
                    .Include(c => c.Synonyms)
                    .FirstOrDefault(c => c.Command.ToLower() == command.ToLower());

                if (commandToDelete != null)
                {
                    context.Answers.RemoveRange(commandToDelete.Answers);
                    context.Synonyms.RemoveRange(commandToDelete.Synonyms);
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
                    .Include(c => c.Synonyms)
                    .FirstOrDefault(c => c.Command.ToLower() == oldCommand.ToLower());

                if (commandToUpdate != null)
                {
                    commandToUpdate.Command = newCommand;

                    // Aktualisiere das Synonym, das dem alten Befehlsnamen entspricht
                    var oldSynonym = commandToUpdate.Synonyms
                        .FirstOrDefault(s => s.Synonym.ToLower() == oldCommand.ToLower());
                    if (oldSynonym != null)
                    {
                        oldSynonym.Synonym = newCommand;
                    }
                    else
                    {
                        // Falls kein entsprechendes Synonym existiert, fügen wir es hinzu
                        commandToUpdate.Synonyms.Add(new SynonymEntry { Synonym = newCommand });
                    }

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

        public void AddSynonym(string command, string synonym)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var commandEntry = context.Commands.FirstOrDefault(c => c.Command.ToLower() == command.ToLower());
                if (commandEntry == null)
                {
                    commandEntry = new CommandEntry { Command = command };
                    context.Commands.Add(commandEntry);
                    context.SaveChanges();
                }

                if (!context.Synonyms.Any(s => s.CommandId == commandEntry.Id && s.Synonym.ToLower() == synonym.ToLower()))
                {
                    context.Synonyms.Add(new SynonymEntry { CommandId = commandEntry.Id, Synonym = synonym });
                    context.SaveChanges();
                }
            }
        }

        public List<string> GetSynonyms(string command)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                return context.Commands
                    .Where(c => c.Command.ToLower() == command.ToLower())
                    .SelectMany(c => c.Synonyms.Select(s => s.Synonym))
                    .ToList();
            }
        }

        public string GetCommandFromSynonym(string synonym)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                return context.Synonyms
                    .Where(s => s.Synonym.ToLower() == synonym.ToLower())
                    .Select(s => s.Command.Command)
                    .FirstOrDefault();
            }
        }

        public void DeleteSynonym(string command, string synonym)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var commandEntry = context.Commands
                    .Include(c => c.Synonyms)
                    .FirstOrDefault(c => c.Command.ToLower() == command.ToLower());

                if (commandEntry != null)
                {
                    var synonymToDelete = commandEntry.Synonyms
                        .FirstOrDefault(s => s.Synonym.ToLower() == synonym.ToLower());

                    if (synonymToDelete != null && commandEntry.Synonyms.Count > 1)
                    {
                        context.Synonyms.Remove(synonymToDelete);
                        context.SaveChanges();
                    }
                }
            }
        }

        public string GetRandomAnswer(string command)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var answers = context.Commands
                    .Where(c => c.Command.ToLower() == command.ToLower())
                    .SelectMany(c => c.Answers.Select(a => a.Answer))
                    .ToList();

                if (answers.Any())
                {
                    Random rnd = new Random();
                    int index = rnd.Next(answers.Count);
                    return answers[index];
                }

                return null;
            }
        }

        public bool HasMinimumSynonyms(string command, int minSynonyms = 5)
        {
            using (var context = new CommandDbContext(_connectionString))
            {
                var synonymCount = context.Commands
                    .Where(c => c.Command.ToLower() == command.ToLower())
                    .SelectMany(c => c.Synonyms)
                    .Count();

                return synonymCount >= minSynonyms;
            }
        }

        public void EnsureMinimumSynonyms(string command, int minSynonyms = 5)
        {
            if (!HasMinimumSynonyms(command, minSynonyms))
            {
                var currentSynonyms = GetSynonyms(command);
                var missingCount = minSynonyms - currentSynonyms.Count;

                for (int i = 0; i < missingCount; i++)
                {
                    AddSynonym(command, $"{command}_synonym_{i + 1}");
                }
            }
        }
    }
}