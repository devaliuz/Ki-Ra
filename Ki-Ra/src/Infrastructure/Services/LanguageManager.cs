using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;

namespace KiRa.Infrastructure.Services
{
    public class LanguageManager
    {
        private static ResourceManager _resourceManager;

        static LanguageManager()
        {
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;

            // Namespace und Basisname anpassen
            string baseName = "Ki-Ra.src.Language.texts";
            string resourceName = $"{baseName}_{currentCulture.TwoLetterISOLanguageName}";
            _resourceManager = new ResourceManager(resourceName, typeof(LanguageManager).Assembly);
        }

        public static string GetString(string key)
        {
            return _resourceManager.GetString(key);
        }
    }
}
