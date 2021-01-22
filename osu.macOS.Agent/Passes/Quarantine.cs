namespace osu.macOS.Agent.Passes
{
	public abstract class Quarantine : IPass
	{
		protected abstract string Name { get; }
		protected abstract string Relative { get; }

		private const string Attribute = "com.apple.quarantine";
		private const string Command = "xattr";

		public Entry Apply(Instance instance)
		{
			var data = Instance.ExecuteCommand($"{Command} '{instance.rootPath}{Relative}'");
			if (data.Length == 0) return new Entry($"Quarantine attribute is absent ({Name})");
			return data.Contains(Attribute)
				? new Entry($"Quarantine attribute is present ({Name})", Severity.Error)
				: new Entry($"Unexpected attribute data ({Name}):\n{data}", Severity.Warning);
		}

		public Status Repair(Instance instance)
		{
			Instance.ExecuteCommand($"{Command} -c '{instance.rootPath}{Relative}'");
			return Apply(instance).severity != Severity.None ? Status.Failed : Status.Fixed;
		}
	}

	public class QuarantineBundle : Quarantine
	{
		protected override string Name => "bundle";
		protected override string Relative => "";
	}

	public class QuarantineWrapper : Quarantine
	{
		protected override string Name => "wrapper";
		protected override string Relative => "/Wineskin.app";
	}
}