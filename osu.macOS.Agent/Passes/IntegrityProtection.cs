using Foundation;

namespace osu.macOS.Agent.Passes
{
	public class IntegrityProtection : IPass
	{
		private const string Command = "csrutil status";
		private static readonly NSOperatingSystemVersion CatalinaTable = new NSOperatingSystemVersion(10, 15, 4);

		public Entry Apply(Instance instance)
		{
			var enabled = Instance.ExecuteCommand(Command).Contains("enabled");
			if (!enabled) return new Entry("System Integrity Protection is disabled");

			var details = "System Integrity Protection is enabled";
			var table = NSProcessInfo.ProcessInfo.IsOperatingSystemAtLeastVersion(CatalinaTable);
			if (!Instance.RequiresLongArchitecture() || table) return new Entry(details);
			details += " and must be disabled for 64 bit Wine engines prior to macOS 10.15.4";
			return new Entry(details, Severity.Error);
		}

		public Status Repair(Instance instance) => Status.None;
	}
}