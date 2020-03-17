using Foundation;

namespace osu.macOS.Agent.Passes
{
	public class DetectDirect3D : IPass
	{
		private static readonly NSOperatingSystemVersion HighSierra = new NSOperatingSystemVersion(10, 13, 0);
		private const string TargetKey = "Try To Use GPU Info";

		public Entry Apply(Instance instance)
		{
			var enabled = (bool) (NSNumber) instance.configuration[TargetKey];
			if (!enabled) return new Entry("Detect Direct3D is disabled");

			var severity = NSProcessInfo.ProcessInfo
				.IsOperatingSystemAtLeastVersion(HighSierra)
				? Severity.Error
				: Severity.Warning;
			return new Entry("Detect Direct3D is enabled", severity);
		}

		public Status Repair(Instance instance)
		{
			instance.configuration[TargetKey] = (NSNumber) false;
			return Status.Fixed;
		}
	}
}