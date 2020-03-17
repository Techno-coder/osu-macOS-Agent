using System.IO;
using System.Linq;

namespace osu.macOS.Agent.Passes
{
	internal static class Logging
	{
		private static readonly string[] Candidates = {"Exception", "FAIL", "MISMATCH"};

		public static Entry Apply(Instance instance, string name)
		{
			if (!File.Exists(instance.LoggingPath(name)))
				return new Entry($"Logging file: {name} is absent");

			var lines = File.ReadLines(instance.LoggingPath(name))
				.Where(line => Candidates.Any(line.Contains)).ToArray();
			if (lines.Length == 0) return new Entry("No errors in: " + name);

			var details = "Errors in: " + name;
			details = lines.Aggregate(details, (current, line) => current + ('\n' + line));
			return new Entry(details, Severity.Warning);
		}
	}

	public class UpdateLogging : IPass
	{
		public Entry Apply(Instance instance) => Logging.Apply(instance, "update.log");

		public Status Repair(Instance instance) => Status.None;
	}

	public class RuntimeLogging : IPass
	{
		public Entry Apply(Instance instance) => Logging.Apply(instance, "runtime.log");

		public Status Repair(Instance instance) => Status.None;
	}
}