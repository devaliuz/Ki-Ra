using Microsoft.Extensions.Configuration;
using Vosk;


namespace KiRa.Infrastructure.Services
{
    public class ModelLoaderService
    {
        private readonly string _modelPath;

        public ModelLoaderService(string modelPath)
        {
            _modelPath = modelPath;
        }

        public async Task<Model> LoadModelAsync()
        {
            if (!Directory.Exists(_modelPath))
            {
                throw new DirectoryNotFoundException($"Das Sprachmodell-Verzeichnis wurde nicht gefunden: {_modelPath}");
            }
            return await Task.Run(() =>
            {
                try
                {
                    return new Model(_modelPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Fehler beim Laden des Modells: {ex.Message}");
                    Console.WriteLine($"StackTrace: {ex.StackTrace}");
                    throw;
                }
            });
        }
    }
}