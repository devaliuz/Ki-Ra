using System.Speech.Synthesis;

namespace KiRa.Core.Services
{
    public class TextToSpeechService_SystemSpeechSynthesis
    {
        private readonly SpeechSynthesizer _synthesizer;

        public TextToSpeechService_SystemSpeechSynthesis()
        {
            _synthesizer = new SpeechSynthesizer();
        }

        public void Speak(string text)
        {
            _synthesizer.Speak(text);
        }
    }
}