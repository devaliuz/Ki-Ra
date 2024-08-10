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
        Console.WriteLine("Willkommen! Die App wird gestartet. Bitte haben Sie einen Moment Geduld.");

        var audioPlayerService = new AudioPlayerService();
        var backgroundMusicPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "background_music.mp3");
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
            throw new DirectoryNotFoundException($"Das Modellverzeichnis wurde nicht gefunden: {modelsDirectory}");
        }

        var modelDirectories = Directory.GetDirectories(modelsDirectory);

        string selectedModelPath;

        if (modelDirectories.Length == 1)
        {
            selectedModelPath = modelDirectories[0];
            Console.WriteLine($"Automatisch gewähltes Modell: {new DirectoryInfo(selectedModelPath).Name}");
        }
        else
        {
            Console.WriteLine("Welches Model soll geladen werden?");
            for (int i = 0; i < modelDirectories.Length; i++)
            {
                var dirInfo = new DirectoryInfo(modelDirectories[i]);
                var size = GetDirectorySize(dirInfo);
                Console.WriteLine($"{i + 1}. {dirInfo.Name} ({FormatSize(size)})");
            }

            int choice;
            do
            {
                Console.Write($"Bitte wählen Sie ein Modell (1-{modelDirectories.Length}): ");
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > modelDirectories.Length);

            selectedModelPath = modelDirectories[choice - 1];
            Console.WriteLine($"Gewähltes Modell: {new DirectoryInfo(selectedModelPath).Name}");
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
        services.AddSingleton<TextToSpeechService>();
        services.AddSingleton(audioPlayerService);
        services.AddTransient<RecordAndTranscribeCommand>();

        var serviceProvider = services.BuildServiceProvider();

        try
        {
            var textToSpeechService = serviceProvider.GetRequiredService<TextToSpeechService>();
            textToSpeechService.Speak("Hallo ich bin Kira, dein persönlicher KI-Assistent. Ich lade meine Datenbanken und stehe dir gleich zur Verfügung. Bitte hab einen Moment Geduld.");

            var modelLoaderService = serviceProvider.GetRequiredService<ModelLoaderService>();
            var voiceRecognitionService = serviceProvider.GetRequiredService<VoiceRecognitionService>();
            var audioRecordingService = serviceProvider.GetRequiredService<AudioRecordingService>();
            var command = serviceProvider.GetRequiredService<RecordAndTranscribeCommand>();

            Console.WriteLine("Lade Modell...");
            Model model = await modelLoaderService.LoadModelAsync();
            Console.WriteLine("Modell erfolgreich geladen.");

            Console.WriteLine("Erstelle Recognizer...");
            VoskRecognizer recognizer = new VoskRecognizer(model, 16000.0f);
            Console.WriteLine("Recognizer erfolgreich erstellt.");

            Console.WriteLine("Initialisiere VoiceRecognitionService...");
            voiceRecognitionService.Initialize(recognizer);
            Console.WriteLine("VoiceRecognitionService initialisiert.");

            audioPlayerService.StopBackgroundMusic();
            textToSpeechService.Speak("Die App ist jetzt bereit.");

            await command.ExecuteAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ein Fehler ist aufgetreten: {ex.GetType().Name}");
            Console.WriteLine($"Nachricht: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
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