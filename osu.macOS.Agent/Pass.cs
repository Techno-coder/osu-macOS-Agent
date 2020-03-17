using osu.macOS.Agent.Passes;

namespace osu.macOS.Agent
{
	public static class Pass
	{
		public static readonly IPass[] Passes =
		{
			new SystemVersion(),
			new EngineVersion(),
			new WrapperVersion(),
			new ReleaseStream(),

			new Quarantine(),
			new DetectDirect3D(),
			new CompatibilityMode()
		};
	}

	public interface IPass
	{
		Entry Apply(Instance instance);
		Status Repair(Instance instance);
	}

	public class Entry
	{
		public readonly Severity severity;
		public Status status = Status.None;
		public readonly string details;

		public Entry(string details, Severity severity = Severity.None)
		{
			this.severity = severity;
			this.details = details;
		}

		public override string ToString()
		{
			var display = "";
			if (severity != Severity.None)
				display += $"[{severity}]";
			if (status != Status.None)
				display += $"[{status}]";
			if (display != "")
				display += ' ';
			display += details;
			return display;
		}
	}

	public enum Severity
	{
		None,
		Warning,
		Error
	}

	public enum Status
	{
		None,
		Failed,
		Fixed,
	}
}