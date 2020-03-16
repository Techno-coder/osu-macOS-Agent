using System.Diagnostics;
using System.IO;
using Foundation;

namespace osu.macOS.Agent
{
	public class Instance
	{
		public readonly string rootPath;
		public NSDictionary configuration;

		public Instance(string rootPath)
		{
			this.rootPath = rootPath;
		}

		public string DataPath() => Path.Combine(rootPath, "drive_c/osu!");
		private string ConfigurationPath() => Path.Combine(rootPath, "Contents/Info.plist");

		public string EngineVersion()
		{
			var path = Path.Combine(rootPath, "Contents/Frameworks/wswine.bundle/version");
			return File.Exists(path) ? File.ReadAllText(path).Trim() : null;
		}

		public void Load()
		{
			configuration = NSDictionary.FromFile(ConfigurationPath());
		}

		public void Save()
		{
			configuration.WriteToFile(ConfigurationPath(), true);
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
	}
}