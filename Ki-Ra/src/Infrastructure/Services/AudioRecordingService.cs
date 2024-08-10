using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KiRa.Infrastructure.Services
{
    public class AudioRecordingService
    {
        public async Task<byte[]> RecordAudioAsync()
        {
            var audioData = new List<byte>();
            using (var waveIn = new WaveInEvent())
            {
                var recordingCompletionSource = new TaskCompletionSource<bool>();

                waveIn.WaveFormat = new WaveFormat(16000, 1);
                var isRecording = true;

                waveIn.DataAvailable += (sender, e) =>
                {
                    if (isRecording)
                    {
                        audioData.AddRange(e.Buffer.Take(e.BytesRecorded));
                    }
                };

                waveIn.StartRecording();

                Console.WriteLine("Aufnahme läuft. Drücken Sie eine beliebige Taste, um die Aufnahme zu beenden...");

                await Task.Run(() =>
                {
                    while (!Console.KeyAvailable)
                    {
                        Task.Delay(100).Wait();
                    }
                });

                Console.ReadKey(true);

                isRecording = false;
                waveIn.StopRecording();
            }

            return audioData.ToArray();
        }
    }
}