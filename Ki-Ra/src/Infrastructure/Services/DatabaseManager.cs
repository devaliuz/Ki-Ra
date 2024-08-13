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
                    InitializeDefaultCommands();
                    InitializeSpecialCommands();
                }
            }
        }

        private void InitializeDefaultCommands()
        {
            // Hallo
            AddCommandWithSynonymsAndAnswers(
                LanguageManager.GetString("COMMAND_hallo"),
                new List<string>
                {
                LanguageManager.GetString("COMMAND_hallo"),
                LanguageManager.GetString("COMMAND_SYN_hallo_hi"),
                LanguageManager.GetString("COMMAND_SYN_hallo_hey"),
                LanguageManager.GetString("COMMAND_SYN_hallo_gruessgott"),
                LanguageManager.GetString("COMMAND_SYN_hallo_gutentag"),
                LanguageManager.GetString("COMMAND_SYN_hallo_servus"),
                LanguageManager.GetString("COMMAND_SYN_hallo_moin"),
                LanguageManager.GetString("COMMAND_SYN_hallo_gruezi"),
                LanguageManager.GetString("COMMAND_SYN_hallo_sali")
                },
                new List<string>
                {
                LanguageManager.GetString("COMMAND_ANSW_hallo_1"),
                LanguageManager.GetString("COMMAND_ANSW_hallo_2"),
                LanguageManager.GetString("COMMAND_ANSW_hallo_3"),
                LanguageManager.GetString("COMMAND_ANSW_hallo_4"),
                LanguageManager.GetString("COMMAND_ANSW_hallo_5"),
                LanguageManager.GetString("COMMAND_ANSW_hallo_6"),
                LanguageManager.GetString("COMMAND_ANSW_hallo_7")
                });

            // Wie geht es dir?
            AddCommandWithSynonymsAndAnswers(
                LanguageManager.GetString("COMMAND_wie_geht_es_dir"),
                new List<string>
                {
                LanguageManager.GetString("COMMAND_wie_geht_es_dir"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_wiegehts"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_allesklar"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_wielaeufts"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_wiefuehlstdu"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_stimmung"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_gutdrauf"),
                LanguageManager.GetString("COMMAND_SYN_wie_geht_es_dir_allesgut")
                },
                new List<string>
                {
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_1"),
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_2"),
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_3"),
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_4"),
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_5"),
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_6"),
                LanguageManager.GetString("COMMAND_ANSW_wie_geht_es_dir_7")
                });

            // Erzähle einen Witz
            AddCommandWithSynonymsAndAnswers(
                LanguageManager.GetString("COMMAND_erzaehle_einen_witz"),
                new List<string>
                {
                LanguageManager.GetString("COMMAND_erzaehle_einen_witz"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_erzaehl"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_witz"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_witze"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_scherz"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_joke"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_etwas_lustiges"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_zum_lachen"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_sei_lustig"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_mach_einen_witz"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_bring_mich_zum_lachen"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_sei_witzig"),
                LanguageManager.GetString("COMMAND_SYN_erzaehle_einen_witz_erzaehle_etwas_komisches")
                },
                new List<string>
                {
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_1"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_2"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_3"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_4"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_5"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_6"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_7"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_8"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_9"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_10"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_11"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_12"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_13"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_14"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_15"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_16"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_17"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_18"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_19"),
                LanguageManager.GetString("COMMAND_ANSW_erzaehle_einen_witz_20")
                });

            // Auf Wiedersehen
            AddCommandWithSynonymsAndAnswers(
                LanguageManager.GetString("COMMAND_auf_wiedersehen"),
                new List<string>
                {
                LanguageManager.GetString("COMMAND_auf_wiedersehen"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_tschuess"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_bis_bald"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_ciao"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_adieu"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_machs_gut"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_bis_dann"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_leb_wohl"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_auf_wiederhoeren"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_bis_zum_naechsten_mal"),
                LanguageManager.GetString("COMMAND_SYN_auf_wiedersehen_schoenen_tag_noch")
                },
                new List<string>
                {
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_1"),
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_2"),
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_3"),
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_4"),
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_5"),
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_6"),
                LanguageManager.GetString("COMMAND_ANSW_auf_wiedersehen_7")
                });
        }

        private void InitializeSpecialCommands()
        {
            // Befehle verwalten
            AddCommandWithSynonyms(
                LanguageManager.GetString("COMMAND_befehle_verwalten"),
                new List<string>
                {
                    LanguageManager.GetString("COMMAND_befehle_verwalten"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_hinzufuegen"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_loeschen"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_aendern"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_kommandos"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_kommando_hinzufuegen"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_kommando_loeschen"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_kommando_aendern"),
                    LanguageManager.GetString("COMMAND_SYN_befehle_verwalten_verwaltung")
                });

            // Was kannst du
            AddCommandWithSynonyms(
                LanguageManager.GetString("COMMAND_was_kannst_du"),
                new List<string>
                {                
                    LanguageManager.GetString("COMMAND_was_kannst_du"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_hilfe"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_funktionen"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_faehigkeiten"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_deinefunktionen"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_zeigefaehigkeiten"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_wasmachstdu"),
                    LanguageManager.GetString("COMMAND_SYN_was_kannst_du_wiehelfen")
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

        //public string GetRandomAnswer(string command)
        //{
        //    using (var context = new CommandDbContext(_connectionString))
        //    {
        //        var answers = context.Commands
        //            .Where(c => c.Command.ToLower() == command.ToLower())
        //            .SelectMany(c => c.Answers.Select(a => a.Answer))
        //            .ToList();

        //        if (answers.Any())
        //        {
        //            Random rnd = new Random();
        //            int index = rnd.Next(answers.Count);
        //            return answers[index];
        //        }

        //        return null;
        //    }
        //}

        //public bool HasMinimumSynonyms(string command, int minSynonyms = 5)
        //{
        //    using (var context = new CommandDbContext(_connectionString))
        //    {
        //        var synonymCount = context.Commands
        //            .Where(c => c.Command.ToLower() == command.ToLower())
        //            .SelectMany(c => c.Synonyms)
        //            .Count();

        //        return synonymCount >= minSynonyms;
        //    }
        //}

        //public void EnsureMinimumSynonyms(string command, int minSynonyms = 5)
        //{
        //    if (!HasMinimumSynonyms(command, minSynonyms))
        //    {
        //        var currentSynonyms = GetSynonyms(command);
        //        var missingCount = minSynonyms - currentSynonyms.Count;

        //        for (int i = 0; i < missingCount; i++)
        //        {
        //            AddSynonym(command, $"{command}_synonym_{i + 1}");
        //        }
        //    }
        //}
    }
}