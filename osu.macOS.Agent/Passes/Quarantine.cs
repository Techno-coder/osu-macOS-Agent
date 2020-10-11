namespace osu.macOS.Agent.Passes
{
	public class Quarantine : IPass
	{
		private const string Attribute = "com.apple.quarantine";
		private const string Name = "xattr";

		public Entry Apply(Instance instance)
		{
			var data = Instance.ExecuteCommand($"{Name} '{instance.rootPath}'");
			if (data.Length == 0) return new Entry("Quarantine attribute is absent");
			return data.Contains(Attribute)
				? new Entry("Quarantine attribute is present", Severity.Error)
				: new Entry($"Unexpected attribute data:\n{data}", Severity.Warning);
		}

		public Status Repair(Instance instance)
		{
			Instance.ExecuteCommand($"{Name} -c '{instance.rootPath}'");
			return Apply(instance).severity != Severity.None ? Status.Failed : Status.Fixed;
		}
	}
}