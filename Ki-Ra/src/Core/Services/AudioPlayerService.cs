using NAudio.Wave;
using System;
using System.Threading.Tasks;

namespace KiRa.Core.Services
{
    public class AudioPlayerService : IDisposable
    {
        private WaveOutEvent outputDevice;
        private AudioFileReader audioFile;

        public void PlayBackgroundMusic(string filePath)
        {
            outputDevice = new WaveOutEvent();
            audioFile = new AudioFileReader(filePath);
            outputDevice.Init(audioFile);
            outputDevice.Play();
        }

        public void StopBackgroundMusic()
        {
            outputDevice?.Stop();
        }

        public void PlaySound(string filePath)
        {
            using (var audioFile = new AudioFileReader(filePath))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        public void Dispose()
        {
            outputDevice?.Dispose();
            audioFile?.Dispose();
        }
    }
}