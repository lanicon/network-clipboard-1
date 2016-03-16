using System;
using System.IO;
using Eto.Forms;

namespace NetworkClipboard
{
	public static class Program
	{
        public static Configuration Config;

        private static string configPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            // hacky? way to get root namespace independent of program file name
            typeof(Program).Namespace.ToString().ToLower(),
            "config.json");

		[STAThread]
		public static void Main(string[] args)
		{
            try
            {
                Config = ParseArgs(args);
                new Eto.Forms.Application().Run(new MainWindow());
                Config.Save(configPath);
            }
            catch (InvalidConfigurationException)
            {
                Console.Error.WriteLine(
                    "Invalid configuration. Maybe check the JSON syntax? " +
                    "You can also delete the config file and let it regenerate.");
                Environment.Exit(1);
            }
		}

        private static void ShowHelp()
        {
            Console.WriteLine("Network clipboard");
            Console.WriteLine();
            Console.WriteLine("-c --config <path> Change configuration file location");
            Console.WriteLine();
        }

        private static bool CheckAccess(string path)
        {
            try
            {
                File.GetAccessControl(path);
                return true;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        private static Configuration ParseArgs(string[] args)
        {
            if (args.Length == 2)
            {
                string option = args[0];
                string path = args[1];

                if (option == "-c" || option == "--config" &&
                    File.Exists(path) && CheckAccess(path))
                {
                    configPath = path;
                    return Configuration.Load(path);
                }
            }
            else if (args.Length == 0)
            {
                if (File.Exists(configPath))
                {
                    return Configuration.Load(configPath);
                }
                else
                {
                    return Configuration.GetDefault();
                }
            }

            ShowHelp();
            Environment.Exit(1);
            return null;
        }
	}
}

