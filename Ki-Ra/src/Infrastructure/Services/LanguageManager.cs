using System;
using System.Globalization;
using System.Resources;
using System.Threading;
using System.Reflection;
using System.Collections;

namespace KiRa.Infrastructure.Services
{
    public class LanguageManager
    {
        private static ResourceManager _resourceManager;
        private static CultureInfo _currentCulture;

        static LanguageManager()
        {
            // Korrigierter Ressourcenpfad
            _currentCulture = CultureInfo.CurrentCulture;
            _resourceManager = new ResourceManager("Ki_Ra.src.Language.texts", Assembly.GetExecutingAssembly());
            SetLanguage(_currentCulture);
            //ListAllKeys();
        }

        private static CultureInfo GetPreferredCulture()
        {
            var systemCulture = CultureInfo.CurrentCulture;
            var uiCulture = CultureInfo.CurrentUICulture;

            Console.WriteLine($"System Culture: {systemCulture.Name}");
            Console.WriteLine($"Current UI Culture: {uiCulture.Name}");

            // Prüfe zuerst, ob eine der Kulturen Deutsch ist
            if (systemCulture.TwoLetterISOLanguageName == "de" ||
                uiCulture.TwoLetterISOLanguageName == "de")
            {
                return new CultureInfo("de-DE");
            }

            // Wenn keine deutsche Kultur gefunden wurde, verwende die UI-Kultur
            return uiCulture;
        }

        public static void SetLanguage(CultureInfo culture)
        {
            try
            {
                // Versuche, die Ressourcen für die angegebene Kultur zu laden
                var testString = _resourceManager.GetString("INFO_UI_Language", culture);
                if (testString != null)
                {
                    _currentCulture = culture;
                }
                else
                {
                    // Wenn die Ressourcen nicht gefunden wurden, verwende Englisch
                    _currentCulture = new CultureInfo("en-US");
                }
            }
            catch (Exception ex)
            {
                // Bei einem Fehler verwende Englisch
                _currentCulture = new CultureInfo("en-US");
            }

            // Setze die Kultur für den aktuellen Thread
            Thread.CurrentThread.CurrentUICulture = _currentCulture;
            Thread.CurrentThread.CurrentCulture = _currentCulture;

            Console.WriteLine($"{GetString("INFO_UI_Language")}");
        }

        public static void ListAllKeys()
        {
            try
            {
                ResourceSet resourceSet = _resourceManager.GetResourceSet(_currentCulture, true, true);
                Console.WriteLine("Verfügbare Schlüssel in der Ressourcendatei:");
                foreach (DictionaryEntry entry in resourceSet)
                {
                    Console.WriteLine($"- {entry.Key}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Auflisten der Schlüssel: {ex.Message}");
            }
        }

        public static string GetString(string key)
        {
            try
            {
                string value = _resourceManager.GetString(key, _currentCulture);
                if (string.IsNullOrEmpty(value))
                {
                    // Wenn der String nicht gefunden wurde, versuche es mit Englisch
                    value = _resourceManager.GetString(key, new CultureInfo("en-US"));
                    if (string.IsNullOrEmpty(value))
                    {
                        // Wenn immer noch kein Wert gefunden wurde, gib den Schlüssel zurück
                        Console.WriteLine($"Schlüssel nicht gefunden: {key}");
                        return $"[{key}]";
                    }
                }
                return value;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Abrufen des Strings für den Schlüssel '{key}': {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                return $"[{key}]";
            }
        }

        //public static CultureInfo GetCurrentCulture()
        //{
        //    _currentCulture = CultureInfo.CurrentCulture;

        //    return _currentCulture;
        //}
    }
}