using Foundation;

namespace osu.macOS.Agent.Passes
{
	public class DetectDirect3D : IPass
	{
		private const string TargetKey = "Try To Use GPU Info";

		public Entry Apply(Instance instance) =>
			(bool) (NSNumber) instance.configuration[TargetKey]
				? new Entry("Detect Direct3D is enabled", Severity.Error)
				: new Entry("Detect Direct3D is disabled");

		public Status Repair(Instance instance)
		{
			instance.configuration[TargetKey] = (NSNumber) false;
			return Status.Fixed;
		}
	}
}