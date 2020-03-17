using AppKit;
using Foundation;

namespace osu.macOS.Agent
{
	[Register("AppDelegate")]
	public class AppDelegate : NSApplicationDelegate
	{
		public override bool ApplicationShouldTerminateAfterLastWindowClosed(NSApplication _) => true;
	}
}