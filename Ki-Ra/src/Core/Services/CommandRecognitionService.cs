using Ki_Ra.src.Core.Services;
using KiRa.Infrastructure.Services;

namespace KiRa.Core.Services
{
    public class CommandRecognitionService
    {
        private readonly DatabaseManager _databaseManager;

        public CommandRecognitionService(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
            //_jaroWinkler = new JaroWinkler();
        }

        public string RecognizeCommand(string input)
        {

            // Direkte Übereinstimmung prüfen
            var directCommand = _databaseManager.GetCommandFromSynonym(input.ToLower());
            if (directCommand != null)
            {
                return directCommand;
            }

            // Fuzzy matching mit Levenshtein-Distanz, wenn keine direkte Übereinstimmung gefunden wurde
            var commands = _databaseManager.GetCommands();
            var bestMatch = commands
                .SelectMany(command => _databaseManager.GetSynonyms(command)
                    .Select(synonym => new { Command = command, Synonym = synonym }))
                .Select(x => new
                {
                    x.Command,
                    Similarity = StringSimilarity.CalculateSimilarity(input.ToLower(), x.Synonym.ToLower())
                })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            // Prüfen, ob die beste Übereinstimmung mindestens 50% Ähnlichkeit hat
            if (bestMatch != null && bestMatch.Similarity >= 50)
            {
                return bestMatch.Command;
            }

            // Wenn keine Übereinstimmung gefunden wurde
            return null;
        }
    }
}