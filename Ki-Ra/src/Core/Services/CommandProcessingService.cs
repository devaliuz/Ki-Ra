﻿using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using KiRa.Infrastructure.Services;
using KiRa.Core.Interfaces;

namespace KiRa.Core.Services
{
    public class CommandProcessingService
    {
        private readonly DatabaseManager _databaseManager;
        private readonly TextToSpeechService _textToSpeechService;
        private readonly AudioRecordingService _audioRecordingService;
        private readonly IVoiceRecognitionService _voiceRecognitionService;
        private readonly Random _random;

        public CommandProcessingService(
            DatabaseManager databaseManager,
            TextToSpeechService textToSpeechService,
            AudioRecordingService audioRecordingService,
            IVoiceRecognitionService voiceRecognitionService)
        {
            _databaseManager = databaseManager;
            _textToSpeechService = textToSpeechService;
            _audioRecordingService = audioRecordingService;
            _voiceRecognitionService = voiceRecognitionService;
            _random = new Random();
        }

        public async Task<string> ProcessCommandAsync(string input)
        {
            string lowercaseInput = input.ToLower().Trim();
            Console.WriteLine($"Erkannter Befehl: {lowercaseInput}"); // Debugging-Ausgabe

            // Exakte Übereinstimmung für spezielle Befehle
            switch (lowercaseInput)
            {
                case "neuer befehl":
                    return await AddNewCommandAsync();
                case "was kannst du":
                    return ListAllCommands();
            }

            // Teilweise Übereinstimmung für reguläre Befehle
            var commands = _databaseManager.GetRegularCommands();
            foreach (var command in commands)
            {
                if (lowercaseInput.Contains(command.ToLower()))
                {
                    var answers = _databaseManager.GetAnswers(command);
                    if (answers.Any())
                    {
                        return answers[_random.Next(answers.Count)];
                    }
                }
            }

            return "Entschuldigung, ich habe das nicht verstanden.";
        }

        private string ListAllCommands()
        {
            var commands = _databaseManager.GetRegularCommands();
            commands.Add("neuer befehl");
            commands.Add("was kannst du");
            var commandList = string.Join(", ", commands);

            var response = $"Ich habe folgende Befehle verinnerlicht: {commandList}";

            Console.WriteLine(response);
            _textToSpeechService.Speak(response);

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
                _textToSpeechService.Speak("Neuer Befehl wurde erfolgreich hinzugefügt.");
                return "Neuer Befehl wurde erfolgreich hinzugefügt.";
            }
            else
            {
                _textToSpeechService.Speak("Vorgang abgebrochen. Der neue Befehl wurde nicht hinzugefügt.");
                return "Vorgang abgebrochen. Der neue Befehl wurde nicht hinzugefügt.";
            }
        }

        private async Task<string> GetAudioInputAsync(string prompt)
        {
            _textToSpeechService.Speak(prompt);
            Console.WriteLine(prompt);

            var audioData = await _audioRecordingService.RecordAudioAsync();
            string recognizedText = await _voiceRecognitionService.RecognizeSpeechAsync(audioData);

            Console.WriteLine($"Erkannter Text: {recognizedText}");
            return recognizedText;
        }
    }
}