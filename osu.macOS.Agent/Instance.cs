using System.Diagnostics;
using System.IO;
using System.Linq;
using Foundation;

namespace osu.macOS.Agent
{
	public class Instance
	{
		public readonly string rootPath;
		public NSDictionary configuration;
		public GameConfiguration gameConfiguration;
		public GameConfiguration gameUserConfiguration;

		private static readonly NSOperatingSystemVersion Catalina = new NSOperatingSystemVersion(10, 15, 0);

		public Instance(string rootPath)
		{
			this.rootPath = rootPath;
		}

		public string DataPath() => Path.Combine(rootPath, "drive_c/osu!");
		public string LogsPath(string name) => Path.Combine(DataPath(), "Logs/" + name);
		private string ConfigurationPath() => Path.Combine(rootPath, "Contents/Info.plist");
		private string GameConfigurationPath() => Path.Combine(DataPath(), "osu!.cfg");
		private string GameUserConfigurationPath() => Directory.GetFiles(DataPath(), "osu!.*.cfg").FirstOrDefault();

		public string EngineVersion()
		{
			var path = Path.Combine(rootPath, "Contents/Frameworks/wswine.bundle/version");
			return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
		}

		public void Load()
		{
			configuration = NSDictionary.FromFile(ConfigurationPath());
			gameConfiguration = GameConfiguration.Load(GameConfigurationPath());
			gameUserConfiguration = GameConfiguration.Load(GameUserConfigurationPath());
		}

		public void Save()
		{
			configuration.WriteToFile(ConfigurationPath(), true);
			gameUserConfiguration?.Save(GameUserConfigurationPath());
			gameConfiguration?.Save(GameConfigurationPath());
		}

		public static string ExecuteCommand(string command)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = "/bin/sh",
					Arguments = $"-c \"{command}\"",
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = true,
				}
			};

			process.Start();
			process.WaitForExit();
			return process.StandardOutput.ReadToEnd();
		}

		public static bool RequiresLongArchitecture() =>
			NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(Catalina);
	}
}