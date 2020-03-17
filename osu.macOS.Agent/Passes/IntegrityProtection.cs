namespace osu.macOS.Agent.Passes
{
	public class IntegrityProtection : IPass
	{
		private const string Command = "csrutil status";

		public Entry Apply(Instance instance)
		{
			var enabled = Instance.ExecuteCommand(Command).Contains("enabled");
			if (!enabled) return new Entry("System Integrity Protection is disabled");

			var details = "System Integrity Protection is enabled";
			if (!Instance.RequiresLongArchitecture()) return new Entry(details);
			details += " and must be disabled for 64 bit Wine engines";
			return new Entry(details, Severity.Error);
		}

		public Status Repair(Instance instance) => Status.None;
	}
}