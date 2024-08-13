using System;
using System.IO;
using System.Reflection;
using System.Runtime;
using System.Threading.Tasks;
using KiRa.ConsoleApp.Commands;
using KiRa.Core.Interfaces;
using KiRa.Core.Services;
using KiRa.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vosk;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine($"{LanguageManager.GetString("INFO_Initial_Greeting_TXT")}");

        var audioPlayerService = new AudioPlayerService();
        var backgroundMusicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "src","Media", "background_music.mp3");
        audioPlayerService.PlayBackgroundMusic(backgroundMusicPath);

        // Konfiguration laden
        IConfiguration config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        string exePath = Assembly.GetExecutingAssembly().Location;
        string exeDirectory = Path.GetDirectoryName(exePath);
        string modelsDirectory = Path.Combine(exeDirectory, config["ModelPath"]);

        if (!Directory.Exists(modelsDirectory))
        {
            throw new DirectoryNotFoundException($"{LanguageManager.GetString("ERROR_Modeldirectory_not_found")} {modelsDirectory}");
        }

        var modelDirectories = Directory.GetDirectories(modelsDirectory);

        string selectedModelPath;

        if (modelDirectories.Length == 1)
        {
            selectedModelPath = modelDirectories[0];
            Console.WriteLine($"{LanguageManager.GetString("INFO_Automatically_chosen_Model")} {new DirectoryInfo(selectedModelPath).Name}");
        }
        else
        {
            Console.WriteLine($"{LanguageManager.GetString("INPUT_Chose_Model")}");
            for (int i = 0; i < modelDirectories.Length; i++)
            {
                var dirInfo = new DirectoryInfo(modelDirectories[i]);
                var size = GetDirectorySize(dirInfo);
                Console.WriteLine($"{i + 1}. {dirInfo.Name} ({FormatSize(size)})");
            }

            int choice;
            do
            {
                Console.Write($"{LanguageManager.GetString("INPUT_Chose_Model_Range")}{modelDirectories.Length}): ");
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > modelDirectories.Length);

            selectedModelPath = modelDirectories[choice - 1];
            Console.WriteLine($"{LanguageManager.GetString("INFO_chosen_Model")} {new DirectoryInfo(selectedModelPath).Name}");
        }

        // Speicherverwaltung optimieren
        GCSettings.LargeObjectHeapCompactionMode = GCLargeObjectHeapCompactionMode.CompactOnce;
        GC.Collect();
        GC.WaitForPendingFinalizers();

        var services = new ServiceCollection();

        string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "commands.db");
        var databaseManager = new DatabaseManager(dbPath);

        services.AddSingleton(databaseManager);
        services.AddSingleton<ModelLoaderService>(sp => new ModelLoaderService(selectedModelPath));
        services.AddSingleton<VoiceRecognitionService>();
        services.AddSingleton<IVoiceRecognitionService>(sp => sp.GetRequiredService<VoiceRecognitionService>());
        services.AddSingleton<AudioRecordingService>();
        services.AddSingleton<CommandProcessingService>();
        //services.AddSingleton<TextToSpeechService_SystemSpeechSynthesis>();
        services.AddSingleton<TextToSpeechService_CognitiveServices>();
        services.AddSingleton(audioPlayerService);
        services.AddSingleton<CommandRecognitionService>();

        // Modell als Singleton registrieren
        services.AddSingleton<Model>(sp =>
        {
            var modelLoader = sp.GetRequiredService<ModelLoaderService>();
            return modelLoader.LoadModelAsync().Result;
        });

        services.AddTransient<RecordAndTranscribeCommand>();

        var serviceProvider = services.BuildServiceProvider();

        try
        {
            //var textToSpeechService = serviceProvider.GetRequiredService<TextToSpeechService_SystemSpeechSynthesis>();
            var textToSpeechService = serviceProvider.GetRequiredService<TextToSpeechService_CognitiveServices>();

            textToSpeechService.Speak($"{LanguageManager.GetString("INFO_Initial_Greeting_VOICE")}");

            // Lade das Modell einmal
            Console.WriteLine($"{LanguageManager.GetString("INFO_Loading_Model")}");
            var model = serviceProvider.GetRequiredService<Model>();
            Console.WriteLine($"{LanguageManager.GetString("INFO_Model_loaded")}");

            var voiceRecognitionService = serviceProvider.GetRequiredService<VoiceRecognitionService>();
            var audioRecordingService = serviceProvider.GetRequiredService<AudioRecordingService>();
            var command = serviceProvider.GetRequiredService<RecordAndTranscribeCommand>();

            Console.WriteLine($"{LanguageManager.GetString("INFO_Loading_Recognizer")}");
            VoskRecognizer recognizer = new VoskRecognizer(model, 16000.0f);
            Console.WriteLine($"{LanguageManager.GetString("INFO_Recognizer_loaded")}");

            Console.WriteLine($"{LanguageManager.GetString("INFO_Init_Recognitionservice")}");
            voiceRecognitionService.Initialize(recognizer);
            Console.WriteLine($"{LanguageManager.GetString("INFO_Init_Recognitionservice_done")}");

            audioPlayerService.StopBackgroundMusic();
            textToSpeechService.Speak($"{LanguageManager.GetString("INFO_Ready_VOICE")}");

            await command.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{LanguageManager.GetString("ERROR_occured")} {ex.GetType().Name}");
            Console.WriteLine($"{LanguageManager.GetString("ERROR_Message")} {ex.Message}");
            Console.WriteLine($"{LanguageManager.GetString("ERROR_StackTrace")} {ex.StackTrace}");
        }
        finally
        {
            audioPlayerService.Dispose();
        }
    }

    private static long GetDirectorySize(DirectoryInfo directory)
    {
        return directory.GetFiles("*", SearchOption.AllDirectories).Sum(file => file.Length);
    }

    private static string FormatSize(long bytes)
    {
        string[] suffixes = { "B", "KB", "MB", "GB", "TB" };
        int counter = 0;
        decimal number = (decimal)bytes;
        while (Math.Round(number / 1024) >= 1)
        {
            number /= 1024;
            counter++;
        }
        return string.Format("{0:n1} {1}", number, suffixes[counter]);
    }
}