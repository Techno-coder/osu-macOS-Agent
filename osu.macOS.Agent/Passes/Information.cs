using Foundation;

namespace osu.macOS.Agent.Passes
{
	public class SystemVersion : IPass
	{
		public Entry Apply(Instance instance) => new Entry(NSProcessInfo.ProcessInfo.OperatingSystemVersionString);
		public Status Repair(Instance instance) => Status.None;
	}

	public class EngineVersion : IPass
	{
		public Entry Apply(Instance instance) => new Entry("Engine: " + instance.EngineVersion());
		public Status Repair(Instance instance) => Status.None;
	}

	public class WrapperVersion : IPass
	{
		public Entry Apply(Instance instance) => new Entry(instance.configuration["CFBundleVersion"].ToString());
		public Status Repair(Instance instance) => Status.None;
	}
}