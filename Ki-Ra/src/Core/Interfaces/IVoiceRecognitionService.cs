using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

 namespace KiRa.Core.Interfaces
{
    public interface IVoiceRecognitionService
    {
        void Initialize(Vosk.VoskRecognizer recognizer);
        Task<string> RecognizeSpeechAsync(byte[] audioData);
    }
}

