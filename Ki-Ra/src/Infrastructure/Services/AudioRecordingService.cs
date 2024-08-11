using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KiRa.Infrastructure.Services
{
    public class AudioRecordingService
    {
        public async Task<byte[]> RecordAudioWithThresholdAsync(int maxDurationMs, float thresholdDb, int silenceDurationMs)
        {
            var audioData = new List<byte>();
            using (var waveIn = new WaveInEvent())
            {
                var recordingCompletionSource = new TaskCompletionSource<bool>();
                waveIn.WaveFormat = new WaveFormat(16000, 1);
                var isRecording = true;
                var lastLoudSoundTime = DateTime.Now;

                waveIn.DataAvailable += (sender, e) =>
                {
                    if (isRecording)
                    {
                        audioData.AddRange(e.Buffer.Take(e.BytesRecorded));

                        // Berechne den RMS-Wert des aktuellen Audioblocks
                        float rms = CalculateRms(e.Buffer, e.BytesRecorded);
                        float db = 20 * (float)Math.Log10(rms);

                        if (db > thresholdDb)
                        {
                            lastLoudSoundTime = DateTime.Now;
                        }
                        else if ((DateTime.Now - lastLoudSoundTime).TotalMilliseconds > silenceDurationMs)
                        {
                            isRecording = false;
                            waveIn.StopRecording();
                        }
                    }
                };

                waveIn.StartRecording();

                await Task.WhenAny(
                    recordingCompletionSource.Task,
                    Task.Delay(maxDurationMs)
                );

                waveIn.StopRecording();
            }

            return audioData.ToArray();
        }

        private float CalculateRms(byte[] buffer, int bytesRecorded)
        {
            int sum = 0;
            for (int i = 0; i < bytesRecorded; i += 2)
            {
                short sample = (short)((buffer[i + 1] << 8) | buffer[i]);
                sum += sample * sample;
            }
            return (float)Math.Sqrt(sum / (bytesRecorded / 2));
        }
    }
}