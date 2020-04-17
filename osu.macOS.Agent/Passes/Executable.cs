using System.IO;

namespace osu.macOS.Agent.Passes
{
	public class Executable : IPass
	{
		private const string Launcher = "Contents/MacOS/WineskinLauncher";

		public Entry Apply(Instance instance)
		{
			var path = Path.Combine(instance.rootPath, Launcher);
			var present = Instance.ExecuteCommand($"stat -F {path}")[9] == 'x';
			return !present
				? new Entry("Execute flag is absent", Severity.Error)
				: new Entry("Execute flag is present");
		}

		public Status Repair(Instance instance)
		{
			Instance.ExecuteCommand($"chmod -R +x {instance.rootPath}");
			return Status.Fixed;
		}
	}
}