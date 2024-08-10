using System.Speech.Synthesis;

namespace KiRa.Core.Services
{
    public class TextToSpeechService
    {
        private readonly SpeechSynthesizer _synthesizer;

        public TextToSpeechService()
        {
            _synthesizer = new SpeechSynthesizer();
        }

        public void Speak(string text)
        {
            _synthesizer.Speak(text);
        }
    }
}