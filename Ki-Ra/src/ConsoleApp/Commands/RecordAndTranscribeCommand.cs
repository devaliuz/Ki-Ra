using System;
using System.Threading.Tasks;
using KiRa.Core.Interfaces;
using KiRa.Core.Services;
using KiRa.Infrastructure.Services;
using Vosk;
using NAudio.Wave;
using System.IO;

namespace KiRa.ConsoleApp.Commands
{
    public class RecordAndTranscribeCommand
    {
        private readonly IVoiceRecognitionService _voiceRecognitionService;
        private readonly AudioRecordingService _audioRecordingService;
        private readonly CommandProcessingService _commandProcessingService;
        private readonly TextToSpeechService _textToSpeechService;
        private readonly VoskRecognizer _triggerRecognizer;
        private readonly AudioPlayerService _audioPlayerService;

        public RecordAndTranscribeCommand(
            IVoiceRecognitionService voiceRecognitionService,
            AudioRecordingService audioRecordingService,
            CommandProcessingService commandProcessingService,
            TextToSpeechService textToSpeechService,
            Model model,
            AudioPlayerService audioPlayerService)
        {
            _voiceRecognitionService = voiceRecognitionService;
            _audioRecordingService = audioRecordingService;
            _commandProcessingService = commandProcessingService;
            _textToSpeechService = textToSpeechService;
            _triggerRecognizer = new VoskRecognizer(model, 16000.0f);
            _triggerRecognizer.SetMaxAlternatives(0);
            _triggerRecognizer.SetWords(true);
            _audioPlayerService = audioPlayerService;
        }

        public async Task ExecuteAsync()
        {
            while (true)
            {
                Console.WriteLine($"{LanguageManager.GetString("INFO_Waiting_for_Trigger")}");
                await WaitForTriggerWord();

                // Spiele Audiosignal ab
                PlayTriggerSound();

                Console.WriteLine($"{LanguageManager.GetString("INFO_Triggered")}");
                var audioData = await _audioRecordingService.RecordAudioWithThresholdAsync(5000, -50, 1000);

                if (audioData.Length > 0)
                {
                    Console.WriteLine($"{LanguageManager.GetString("INFO_Convert_Audio")}");
                    string result = await _voiceRecognitionService.RecognizeSpeechAsync(audioData);
                    string extractedText = JsonUtility.ExtractTextFromJson(result);
                    Console.WriteLine($"{LanguageManager.GetString("INFO_Recognized_Order")} {extractedText}");
                    string response = await _commandProcessingService.ProcessCommandAsync(extractedText);
                    Console.WriteLine($"{LanguageManager.GetString("ANSWER")} {response}");
                    _textToSpeechService.Speak(response);
                }
                else
                {
                    Console.WriteLine($"{LanguageManager.GetString("INFO_No_Voirce_Recognized")}");
                }
            }
        }

        private async Task WaitForTriggerWord()
        {
            using (var waveIn = new WaveInEvent())
            {
                waveIn.WaveFormat = new WaveFormat(16000, 1);
                waveIn.DataAvailable += (sender, e) =>
                {
                    if (_triggerRecognizer.AcceptWaveform(e.Buffer, e.BytesRecorded))
                    {
                        var result = _triggerRecognizer.Result();
                        if (result.ToLower().Contains("kira"))
                        {
                            waveIn.StopRecording();
                        }
                    }
                };

                var tcs = new TaskCompletionSource<bool>();
                waveIn.RecordingStopped += (sender, e) => tcs.TrySetResult(true);

                waveIn.StartRecording();
                await tcs.Task;
            }
        }

        private void PlayTriggerSound()
        {
            string soundPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src", "Media", "pling.mp3");
            _audioPlayerService.PlaySound(soundPath);
        }
    }
}