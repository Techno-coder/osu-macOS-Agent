using System.IO;
using System.Linq;

namespace osu.macOS.Agent.Passes
{
	public abstract class LogFile : IPass
	{
		protected abstract string Name { get; }
		private static readonly string[] Candidates = {"Exception", "FAIL", "MISMATCH"};

		public Entry Apply(Instance instance)
		{
			if (!File.Exists(instance.LogsPath(Name)))
				return new Entry($"Log file: {Name} is absent");

			var lines = File.ReadLines(instance.LogsPath(Name))
				.Where(line => Candidates.Any(line.Contains)).ToArray();
			if (lines.Length == 0) return new Entry("No errors in: " + Name);

			var details = "Errors in: " + Name;
			details = lines.Aggregate(details, (current, line) => current + ('\n' + line));
			return new Entry(details, Severity.Warning);
		}

		public Status Repair(Instance instance) => Status.None;
	}

	public class UpdateLogFile : LogFile
	{
		protected override string Name => "update.log";
	}

	public class RuntimeLogFile : LogFile
	{
		protected override string Name => "runtime.log";
	}
}