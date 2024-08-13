using System;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using KiRa.Infrastructure.Services;
using KiRa.Core.Interfaces;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Globalization;

namespace KiRa.Core.Services
{
    public class CommandProcessingService
    {
        private readonly DatabaseManager _databaseManager;
        private readonly TextToSpeechService_SystemSpeechSynthesis _textToSpeechService;
        //private readonly TextToSpeechService_CognitiveServices _textToSpeechService;
        private readonly AudioRecordingService _audioRecordingService;
        private readonly IVoiceRecognitionService _voiceRecognitionService;
        private readonly CommandRecognitionService _commandRecognitionService;
        private readonly Random _random;

        public CommandProcessingService(
            DatabaseManager databaseManager,
            TextToSpeechService_SystemSpeechSynthesis textToSpeechService,
            //TextToSpeechService_CognitiveServices textToSpeechService,
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
            return LanguageManager.GetString("INFO_Didnt_Understand");
        }

        private async Task<string> HandleCommandManagementAsync()
        {
            string response = LanguageManager.GetString("MANAGE_Commands");
            _textToSpeechService.SpeakAsync(response);
            Console.WriteLine(response);

            string action = await GetAudioInputAsync(LanguageManager.GetString("MANAGE_Ask_For_Answer"));

            if (IsActionMatch(action, "GENERAL_Add"))
            {
                return await AddNewCommandAsync();
            }
            else if (IsActionMatch(action, "GENERAL_Delete"))
            {
                return await DeleteCommandAsync();
            }
            else
            {
                response = LanguageManager.GetString("MANAGE_Didnt_Understand");
                _textToSpeechService.SpeakAsync(response);
                return response;
            }
        }

        private bool IsActionMatch(string action, string resourceKey)
        {
            string localizedAction = LanguageManager.GetString(resourceKey);
            return action.Equals(localizedAction, StringComparison.OrdinalIgnoreCase);
        }



        private string ListAllCommands()
        {
            var commands = _databaseManager.GetRegularCommands();
            var commandList = string.Join(", ", commands);

            var response = $"{LanguageManager.GetString("COMMAND_Help")} {commandList}";
            return response;
        }

        private async Task<string> AddNewCommandAsync()
        {
            string response = $"{LanguageManager.GetString("MANAGE_New_Command")}";
            _textToSpeechService.SpeakAsync(response);
            Console.WriteLine(response);

            string newCommand = await GetAudioInputAsync($"{LanguageManager.GetString("MANAGE_Waiting_for_new_Command")}");

            bool commandExists = _databaseManager.CommandExists(newCommand);
            if (commandExists)
            {
                response = $"{LanguageManager.GetString("MANAGE_Comand_Exists")}";
                _textToSpeechService.SpeakAsync(response);
                Console.WriteLine(response);
            }
            else
            {
                response = $"{LanguageManager.GetString("MANAGE_Adding_New_Command")}";
                _textToSpeechService.SpeakAsync(response);
                Console.WriteLine(response);
            }

            string newAnswer = await GetAudioInputAsync($"{LanguageManager.GetString("MANAGE_Add_Answer")}");

            response = $"{LanguageManager.GetString("MANAGE_Check_Answer")}".Replace("$$$",newAnswer);

            _textToSpeechService.SpeakAsync(response);
            Console.WriteLine(response);

            string confirmation = await GetAudioInputAsync($"{LanguageManager.GetString("MANAGE_Commit_new_Answer")}");

            if (confirmation.ToLower() == "ja")
            {
                _databaseManager.AddNewCommand(newCommand, newAnswer);
                return $"{LanguageManager.GetString("MANAGE_Succsess")}";
            }
            else
            {
                return $"{LanguageManager.GetString("MANAGE_Cancel")}";
            }
        }

        private async Task<string> DeleteCommandAsync()
        {
            string response = $"{LanguageManager.GetString("MANAGE_Delete_Command_Input")}";
            _textToSpeechService.SpeakAsync(response);
            Console.WriteLine(response);

            string commandToDelete = await GetAudioInputAsync($"{LanguageManager.GetString("MANAGE_Delete_Command_CTA")}");

            if (_databaseManager.CommandExists(commandToDelete))
            {
                _databaseManager.DeleteCommand(commandToDelete);
                response = $"{LanguageManager.GetString("MANAGE_Delete_Succsess")}".Replace("$$$", commandToDelete);
                return response;
            }
            else
            {
                response = $"{LanguageManager.GetString("MANAGE_Delete_Cancel")}".Replace("$$$", commandToDelete);
                return response;
            }
        }

        private async Task<string> GetAudioInputAsync(string prompt)
        {
            _textToSpeechService.SpeakAsync(prompt);
            Console.WriteLine(prompt);

            AudioPlayerService audioPlayerService = new AudioPlayerService();

            string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Media", "pling.mp3");

            audioPlayerService.PlaySound(soundPath);

            var audioData = await _audioRecordingService.RecordAudioWithThresholdAsync(5000, -50, 1000);

            string recognizedText = await _voiceRecognitionService.RecognizeSpeechAsync(audioData);

            return JsonUtility.ExtractTextFromJson(recognizedText);
        }
    }
}