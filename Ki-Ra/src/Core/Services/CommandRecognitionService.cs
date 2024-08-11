using System;
using System.Collections.Generic;
using System.Linq;
using F23.StringSimilarity;
using KiRa.Infrastructure.Services;

namespace KiRa.Core.Services
{
    public class CommandRecognitionService
    {
        private readonly DatabaseManager _databaseManager;
        private readonly JaroWinkler _jaroWinkler;
        private const double SimilarityThreshold = 0.8;

        public CommandRecognitionService(DatabaseManager databaseManager)
        {
            _databaseManager = databaseManager;
            _jaroWinkler = new JaroWinkler();
        }

        public string RecognizeCommand(string input)
        {
            // Direkte Übereinstimmung prüfen
            var directCommand = _databaseManager.GetCommandFromSynonym(input.ToLower());
            if (directCommand != null)
            {
                return directCommand;
            }

            // Fuzzy matching, wenn keine direkte Übereinstimmung gefunden wurde
            var commands = _databaseManager.GetCommands();
            var bestMatch = commands
                .SelectMany(command => _databaseManager.GetSynonyms(command)
                    .Select(synonym => new { Command = command, Synonym = synonym }))
                .Select(x => new { x.Command, Similarity = _jaroWinkler.Similarity(input.ToLower(), x.Synonym.ToLower()) })
                .OrderByDescending(x => x.Similarity)
                .FirstOrDefault();

            if (bestMatch != null && bestMatch.Similarity >= SimilarityThreshold)
            {
                return bestMatch.Command;
            }

            // Wenn keine Übereinstimmung gefunden wurde
            return null;
        }
    }
}