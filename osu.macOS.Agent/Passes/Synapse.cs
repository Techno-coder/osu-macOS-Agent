using System.Linq;
using AppKit;

namespace osu.macOS.Agent.Passes
{
	public class Synapse : IPass
	{
		private const string TargetFirst = "Razer";
		private const string TargetSecond = "Synapse";

		public Entry Apply(Instance instance)
		{
			return NSWorkspace.SharedWorkspace.RunningApplications
				.Where(application => application.LocalizedName.Contains(TargetFirst))
				.Any(application => application.LocalizedName.Contains(TargetSecond))
				? new Entry($"{TargetFirst} {TargetSecond} is present and interferes with Wine", Severity.Error)
				: new Entry($"{TargetFirst} {TargetSecond} is absent");
		}

		public Status Repair(Instance instance) => Status.None;
	}
}