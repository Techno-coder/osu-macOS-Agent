namespace osu.macOS.Agent.Passes
{
	public class ReleaseStream : IPass
	{
		private const string Stream = "_ReleaseStream";

		public Entry Apply(Instance instance)
		{
			if (instance.gameConfiguration == null)
				return new Entry("Game configuration is absent");
			var stream = instance.gameConfiguration.GetValue(Stream);
			return stream == "Stable"
				? new Entry("Release: " + stream, Severity.Warning)
				: new Entry("Release: " + stream);
		}

		public Status Repair(Instance instance)
		{
			instance.gameConfiguration.SetValue(Stream, "Stable40");
			return Status.Fixed;
		}
	}
}