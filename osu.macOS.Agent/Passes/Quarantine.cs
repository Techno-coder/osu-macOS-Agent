namespace osu.macOS.Agent.Passes
{
	public class Quarantine : IPass
	{
		private const string Attribute = "com.apple.quarantine";
		private const string Name = "xattr";

		public Entry Apply(Instance instance)
		{
			var data = Instance.ExecuteCommand($"{Name} {instance.rootPath}");
			return data.Contains(Attribute)
				? new Entry("Quarantine attribute is present", Severity.Error)
				: new Entry("Quarantine attribute is absent");
		}

		public Status Repair(Instance instance)
		{
			Instance.ExecuteCommand($"{Name} -dr {Attribute} '{instance.rootPath}'");
			return Status.Fixed;
		}
	}
}