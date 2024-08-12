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
                throw new DirectoryNotFoundException($"{LanguageManager.GetString("ERROR_LangModel_Directory_not_found")} {_modelPath}");
            }
            return await Task.Run(() =>
            {
                try
                {
                    return new Model(_modelPath);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"{LanguageManager.GetString("ERROR_Message")} {ex.Message}");
                    Console.WriteLine($"{LanguageManager.GetString("ERROR_StackTrace")}: {ex.StackTrace}");
                    throw;
                }
            });
        }
    }
}