using KiRa.Core.Interfaces;
using KiRa.Core.Services;
using KiRa.Infrastructure.Services;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace KiRa.ConsoleApp.Commands
{
    public class RecordAndTranscribeCommand
    {
        private readonly IVoiceRecognitionService _voiceRecognitionService;
        private readonly AudioRecordingService _audioRecordingService;
        private readonly CommandProcessingService _commandProcessingService;
        private readonly TextToSpeechService _textToSpeechService;

        public RecordAndTranscribeCommand(
            IVoiceRecognitionService voiceRecognitionService,
            AudioRecordingService audioRecordingService,
            CommandProcessingService commandProcessingService,
            TextToSpeechService textToSpeechService)
        {
            _voiceRecognitionService = voiceRecognitionService;
            _audioRecordingService = audioRecordingService;
            _commandProcessingService = commandProcessingService;
            _textToSpeechService = textToSpeechService;
        }

        public async Task ExecuteAsync()
        {
            while (true)
            {
                Console.WriteLine("\nDrücken Sie eine beliebige Taste, um die Aufnahme zu starten (ESC zum Beenden)...");
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Escape)
                {
                    _textToSpeechService.Speak("Die Anwendung wird beendet. Auf Wiedersehen!");
                    break;
                }

                var audioData = await _audioRecordingService.RecordAudioAsync();

                Console.WriteLine("Aufnahme beendet. Verarbeite Audio...");

                string result = await _voiceRecognitionService.RecognizeSpeechAsync(audioData);

                // JSON-Antwort in Klartext umwandeln
                string extractedText = ExtractTextFromJson(result);
                Console.WriteLine($"Erkannter Text (Klartext): {extractedText}");

                string response = await _commandProcessingService.ProcessCommandAsync(extractedText); // Übergeben Sie den Klartext

                _textToSpeechService.Speak(response);
            }
        }

        private string ExtractTextFromJson(string jsonInput)
        {
            try
            {
                var jsonObject = JObject.Parse(jsonInput);
                return jsonObject["text"].ToString();
            }
            catch (JsonReaderException)
            {
                // Wenn die Eingabe kein gültiges JSON ist, gib sie unverändert zurück
                return jsonInput;
            }
        }
    }
}