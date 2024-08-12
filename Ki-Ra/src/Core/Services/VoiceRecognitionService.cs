using KiRa.Core.Interfaces;
using Vosk;
using System.Threading.Tasks;

namespace KiRa.Core.Services
{
    public class VoiceRecognitionService : IVoiceRecognitionService
    {
        private VoskRecognizer _recognizer;

        public void Initialize(VoskRecognizer recognizer)
        {
            _recognizer = recognizer;
        }

        public Task<string> RecognizeSpeechAsync(byte[] audioData)
        {
            if (_recognizer == null)
            {
                throw new InvalidOperationException($"Recognizer has not been initialized. Call Initialize method first.");
            }

            _recognizer.AcceptWaveform(audioData, audioData.Length);
            string result = _recognizer.FinalResult();
            return Task.FromResult(result);
        }
    }
}