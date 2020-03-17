using System.IO;
using System.Linq;

namespace osu.macOS.Agent
{
	public class GameConfiguration
	{
		private string data;

		public static GameConfiguration Load(string path)
		{
			return File.Exists(path) ? new GameConfiguration {data = File.ReadAllText(path)} : null;
		}

		public string GetValue(string key) =>
			(from line in data.Split('\n')
				where line.StartsWith(key)
				let index = line.IndexOf('=') + 1
				select line.Substring(index).Trim()).FirstOrDefault();

		public void SetValue(string key, string value)
		{
			var replacement = $"{key} = {value}";
			var current = $"{key} = {GetValue(key)}";
			data = data.Replace(current, replacement);
		}

		public void Save(string path)
		{
			File.WriteAllText(path, data);
		}
	}
}