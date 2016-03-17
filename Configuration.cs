using System;
using System.Text;
using System.IO;
using System.Web.Script.Serialization;

namespace NetworkClipboard
{
    public class InvalidConfigurationException : Exception
    { }

    public class Configuration
    {
        public int Port { get; set; }
        public string[] Channels { get; set; }

        // don't serialize this one
        public static bool IsValid = true;

        public Configuration()
        { }

        public void Save(string path)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();

            string unformatted = serializer.Serialize(this);
            string formatted = JsonFormatter.PrettyFormat(unformatted);

            Directory.CreateDirectory(Path.GetDirectoryName(path));
            File.WriteAllText(path, formatted);
        }

        public static Configuration Load(string path)
        {
            try
            {
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                string json = File.ReadAllText(path);
                return serializer.Deserialize<Configuration>(json);
            }
            catch (ArgumentException)
            {
                throw new InvalidConfigurationException();
            }
            catch (InvalidOperationException)
            {
                throw new InvalidConfigurationException();
            }
        }

        public static Configuration GetDefault()
        {
            return new Configuration()
            {
                Port = 45454,
                Channels = new string[]{ "default" }
            };
        }
    }
}

