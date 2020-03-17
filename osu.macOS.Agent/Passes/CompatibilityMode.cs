namespace osu.macOS.Agent.Passes
{
	public class CompatibilityMode : IPass
	{
		private const string CompatibilityContext = "CompatibilityContext";

		public Entry Apply(Instance instance)
		{
			if (instance.gameUserConfiguration == null)
				return new Entry("Game user configuration is absent");
			return instance.gameUserConfiguration.GetValue(CompatibilityContext) == "1"
				? new Entry("Compatibility mode is enabled", Severity.Error)
				: new Entry("Compatibility mode is disabled");
		}

		public Status Repair(Instance instance)
		{
			instance.gameUserConfiguration.SetValue(CompatibilityContext, "0");
			return Status.Fixed;
		}
	}
}