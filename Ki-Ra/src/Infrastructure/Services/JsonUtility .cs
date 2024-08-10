using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KiRa.Infrastructure.Services
{
    public static class JsonUtility
    {
        public static string ExtractTextFromJson(string jsonInput)
        {
            try
            {
                var jsonObject = JObject.Parse(jsonInput);
                return jsonObject["text"]?.ToString() ?? string.Empty;
            }
            catch (JsonReaderException)
            {
                return jsonInput;
            }
        }
    }
}
