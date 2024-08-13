using System;
using System.IO;
using System.Speech.Synthesis;
using System.Threading.Tasks;
using MCS = Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;

namespace KiRa.Core.Services
{
    public class TextToSpeechService_CognitiveServices : IDisposable
    {
        private MCS.SpeechSynthesizer _synthesizer;
        private MCS.SpeechConfig _speechConfig;

        public TextToSpeechService_CognitiveServices()
        {
            InitializeSpeechSynthesizer();
        }

        private void InitializeSpeechSynthesizer()
        {
            // Konfigurieren Sie hier Ihre bevorzugte Stimme und Sprache
            _speechConfig = MCS.SpeechConfig.FromHost(new Uri("https://eastus.tts.speech.microsoft.com/cognitiveservices/v1"));
            _speechConfig.SpeechSynthesisVoiceName = "de-DE-KatjaNeural";

            // Verwenden Sie den Standard-Audioausgang des Systems
            var audioConfig = AudioConfig.FromDefaultSpeakerOutput();

            _synthesizer = new MCS.SpeechSynthesizer(_speechConfig, audioConfig);
        }

        public async Task Speak(string text)
        {
            if (_synthesizer == null)
            {
                throw new InvalidOperationException("Speech synthesizer is not initialized.");
            }

            using (var result = await _synthesizer.SpeakTextAsync(text))
            {
                if (result.Reason == MCS.ResultReason.SynthesizingAudioCompleted)
                {
                    Console.WriteLine($"Speech synthesized for text: [{text}]");
                }
                else if (result.Reason == MCS.ResultReason.Canceled)
                {
                    var cancellation = MCS.SpeechSynthesisCancellationDetails.FromResult(result);
                    Console.WriteLine($"CANCELED: Reason={cancellation.Reason}");

                    if (cancellation.Reason == MCS.CancellationReason.Error)
                    {
                        Console.WriteLine($"CANCELED: ErrorCode={cancellation.ErrorCode}");
                        Console.WriteLine($"CANCELED: ErrorDetails=[{cancellation.ErrorDetails}]");
                    }
                }
            }
        }

        public void Dispose()
        {
            _synthesizer?.Dispose();
            // Entfernen Sie den Aufruf von Dispose für _speechConfig
            _speechConfig = null;
        }
    }
}