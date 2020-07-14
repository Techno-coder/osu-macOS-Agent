using System.Linq;

namespace osu.macOS.Agent.Passes
{
	public class LongEngine : IPass
	{
		private const string Error = "Engine is outdated for systems newer than Catalina and must be updated";

		public Entry Apply(Instance instance)
		{
			var suffix = instance.EngineVersion().Substring(2);
			var version = int.Parse(new string(suffix.TakeWhile(char.IsDigit).ToArray()));
			if (!Instance.RequiresLongArchitecture() || version > 10)
				return new Entry("Engine version is supported");
			return new Entry(Error, Severity.Error);
		}

		public Status Repair(Instance instance) => Status.None;
	}
}