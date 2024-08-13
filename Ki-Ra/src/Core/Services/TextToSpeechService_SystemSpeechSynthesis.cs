using System.Speech.Synthesis;

namespace KiRa.Core.Services
{
    public class TextToSpeechService_SystemSpeechSynthesis
    {
        private readonly SpeechSynthesizer _synthesizer;
        public TextToSpeechService_SystemSpeechSynthesis()
        {
            _synthesizer = new SpeechSynthesizer();
            _synthesizer.SetOutputToDefaultAudioDevice();
        }
        public async Task SpeakAsync(string text)
        {
            await Task.Run(() => _synthesizer.Speak(text));
        }
        public void Dispose()
        {
            _synthesizer?.Dispose();
        }
    }
}