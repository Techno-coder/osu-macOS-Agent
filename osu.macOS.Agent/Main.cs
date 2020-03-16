using AppKit;

namespace osu.macOS.Agent
{
	internal static class MainClass
	{
		private static void Main(string[] arguments)
		{
			NSApplication.Init();
			NSApplication.Main(arguments);
		}
	}
}