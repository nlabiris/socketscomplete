using Newtonsoft.Json;
using System;

namespace SocketsComplete
{
    /// <summary>
    /// Logger for server progress and error logging
    /// </summary>
    internal class Logger
    {
        private string Prefix;
        private JsonSerializerSettings JsonSettings;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="prefix">Logger prefix</param>
        public Logger(string prefix)
        {
            Prefix = prefix;
            JsonSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public void Console(string message)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} {Prefix}: {message}");
        }

        public void Console(Exception exception)
        {
            System.Console.WriteLine($"{DateTime.Now.ToString("dd-MM-yyyy HH:mm:ss")} {Prefix}: {exception}");
        }

        public void Console(ParseResults result)
        {
            var message = $"Parse results:\n";
            message += JsonConvert.SerializeObject(result, JsonSettings) + "\n";
            Console(message);
        }
    }
}
