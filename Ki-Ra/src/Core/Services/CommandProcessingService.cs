using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using KiRa.Infrastructure.Services;
using KiRa.Core.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace KiRa.Core.Services
{
    public class CommandProcessingService
    {
        private readonly DatabaseManager _databaseManager;
        private readonly TextToSpeechService _textToSpeechService;
        private readonly AudioRecordingService _audioRecordingService;
        private readonly IVoiceRecognitionService _voiceRecognitionService;
        private readonly CommandRecognitionService _commandRecognitionService;
        private readonly Random _random;

        public CommandProcessingService(
            DatabaseManager databaseManager,
            TextToSpeechService textToSpeechService,
            AudioRecordingService audioRecordingService,
            IVoiceRecognitionService voiceRecognitionService,
            CommandRecognitionService commandRecognitionService)
        {
            _databaseManager = databaseManager;
            _textToSpeechService = textToSpeechService;
            _audioRecordingService = audioRecordingService;
            _voiceRecognitionService = voiceRecognitionService;
            _commandRecognitionService = commandRecognitionService;
            _random = new Random();
        }

        public async Task<string> ProcessCommandAsync(string input)
        {
            string recognizedCommand = _commandRecognitionService.RecognizeCommand(input);

            if (recognizedCommand != null)
            {
                // Überprüfen Sie zuerst auf die speziellen Befehle
                if (_databaseManager.GetSynonyms("befehle verwalten").Contains(recognizedCommand.ToLower()))
                {
                    return await HandleCommandManagementAsync();
                }
                else if (_databaseManager.GetSynonyms("was kannst du").Contains(recognizedCommand.ToLower()))
                {
                    return ListAllCommands();
                }

                // Wenn es kein spezieller Befehl ist, suchen Sie nach Antworten in der Datenbank
                var answers = _databaseManager.GetAnswers(recognizedCommand);
                if (answers.Any())
                {
                    return answers[_random.Next(answers.Count)];
                }
            }

            // Wenn kein Befehl erkannt wurde oder keine Antwort gefunden wurde
            return "Entschuldigung, ich habe das nicht verstanden.";
        }

        private async Task<string> HandleCommandManagementAsync()
        {
            _textToSpeechService.Speak("Möchtest du neue Befehle oder Antworten hinzufügen, oder vorhandene Befehle löschen?");
            Console.WriteLine("Möchtest du neue Befehle oder Antworten hinzufügen, oder vorhandene Befehle löschen?");

            string action = await GetAudioInputAsync("Bitte sage 'Hinzufügen' oder 'Löschen'.");

            switch (action.ToLower())
            {
                case "hinzufügen":
                    return await AddNewCommandAsync();
                case "löschen":
                    return await DeleteCommandAsync();
                default:
                    _textToSpeechService.Speak("Ich habe dich nicht verstanden. Bitte sage 'Hinzufügen' oder 'Löschen'.");
                    return "Ich habe dich nicht verstanden. Bitte sage 'Hinzufügen' oder 'Löschen'.";
            }
        }

        private string ListAllCommands()
        {
            var commands = _databaseManager.GetRegularCommands();
            var commandList = string.Join(", ", commands);

            var response = $"Ich habe folgende Befehle verinnerlicht: {commandList}";
            return response;
        }

        private async Task<string> AddNewCommandAsync()
        {
            _textToSpeechService.Speak("Okay, lassen Sie uns einen neuen Befehl hinzufügen.");
            Console.WriteLine("Okay, lassen Sie uns einen neuen Befehl hinzufügen.");

            string newCommand = await GetAudioInputAsync("Bitte sagen Sie den Namen des neuen Befehls.");

            bool commandExists = _databaseManager.CommandExists(newCommand);
            if (commandExists)
            {
                _textToSpeechService.Speak("Dieser Befehl existiert bereits. Wir fügen eine neue Antwort hinzu.");
                Console.WriteLine("Dieser Befehl existiert bereits. Wir fügen eine neue Antwort hinzu.");
            }
            else
            {
                _textToSpeechService.Speak("Neuer Befehl wird angelegt.");
                Console.WriteLine("Neuer Befehl wird angelegt.");
            }

            string newAnswer = await GetAudioInputAsync("Bitte sagen Sie die Antwort für diesen Befehl.");

            _textToSpeechService.Speak($"Die neue Antwort lautet: {newAnswer}. Ist das korrekt? Sagen Sie ja, um zu bestätigen.");
            Console.WriteLine($"Die neue Antwort lautet: {newAnswer}. Ist das korrekt? Sagen Sie 'ja', um zu bestätigen.");

            string confirmation = await GetAudioInputAsync("Bitte bestätigen Sie mit 'ja' oder 'nein'.");

            if (confirmation.ToLower() == "ja")
            {
                _databaseManager.AddNewCommand(newCommand, newAnswer);
                return "Neuer Befehl wurde erfolgreich hinzugefügt.";
            }
            else
            {
                return "Vorgang abgebrochen. Der neue Befehl wurde nicht hinzugefügt.";
            }
        }

        private async Task<string> DeleteCommandAsync()
        {
            _textToSpeechService.Speak("Welchen Befehl möchtest du löschen?");
            Console.WriteLine("Welchen Befehl möchtest du löschen?");

            string commandToDelete = await GetAudioInputAsync("Bitte nenne den Befehl.");

            if (_databaseManager.CommandExists(commandToDelete))
            {
                _databaseManager.DeleteCommand(commandToDelete);
                return $"Der Befehl '{commandToDelete}' wurde erfolgreich gelöscht.";
            }
            else
            {
                _textToSpeechService.Speak($"Der Befehl '{commandToDelete}' existiert nicht.");
                return $"Der Befehl '{commandToDelete}' existiert nicht.";
            }
        }

        private async Task<string> GetAudioInputAsync(string prompt)
        {
            Console.WriteLine(prompt);

            AudioPlayerService audioPlayerService = new AudioPlayerService();

            audioPlayerService.PlaySound("pling.mp3");

            var audioData = await _audioRecordingService.RecordAudioWithThresholdAsync(5000, -50, 1000);

            string recognizedText = await _voiceRecognitionService.RecognizeSpeechAsync(audioData);

            return JsonUtility.ExtractTextFromJson(recognizedText);
        }
    }
}