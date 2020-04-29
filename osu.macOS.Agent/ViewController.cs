using System;
using System.IO;
using System.Linq;
using System.Net;
using AppKit;
using Foundation;

namespace osu.macOS.Agent
{
	public partial class ViewController : NSViewController
	{
		private Instance instance;
		private PassTableSource passSource;
		private readonly NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;
		private const string UpdateLocation = "https://m1.ppy.sh/r/osu!install.exe";

		public ViewController(IntPtr handle) : base(handle) {}

		partial void ScanButtonClick(NSObject sender)
		{
			instance.Load();
			passSource = new PassTableSource();
			RunPasses((pass, _) =>
			{
				try
				{
					var entry = pass.Apply(instance);
					passSource.entries.Add(entry);
				}
				catch (Exception exception)
				{
					var entry = new Entry(exception.ToString()) {status = Status.Failed};
					passSource.entries.Add(entry);
				}
			});

			RepairButton.Enabled = true;
			ReportButton.Enabled = true;
			PassTable.DataSource = passSource;
			PassTable.Delegate = new PassTableDelegate(passSource);
		}

		partial void RepairButtonClick(NSObject sender)
		{
			RunPasses((pass, index) =>
			{
				var entry = passSource.entries[index];
				if (entry.severity == Severity.None || entry.status != Status.None) return;

				try
				{
					entry.status = pass.Repair(instance);
				}
				catch (Exception)
				{
					entry.status = Status.Failed;
				}
			});

			PassTable.ReloadData();
			RepairButton.Enabled = false;
			instance.Save();
		}

		partial void ReportButtonClick(NSObject sender)
		{
			var display = passSource.entries.Aggregate("[box=Report]\n",
				(current, entry) => current + (entry.ToString() + '\n'));
			display += "[/box]";

			NSPasteboard.GeneralPasteboard.ClearContents();
			var contents = new INSPasteboardWriting[] {(NSString) display};
			NSPasteboard.GeneralPasteboard.WriteObjects(contents);
		}

		partial void UpdateButtonClick(NSObject sender)
		{
			UpdateButton.Enabled = false;
			var client = new WebClient();
			client.DownloadFileCompleted += (_, arguments) =>
			{
				UpdateButton.Enabled = true;
				UpdateProgress.DoubleValue = 0;
			};

			client.DownloadProgressChanged += (_, arguments) =>
				UpdateProgress.DoubleValue = arguments.ProgressPercentage;
			var target = Path.Combine(instance.DataPath(), "osu!.exe");
			client.DownloadFileAsync(new Uri(UpdateLocation), target);
		}

		public override void ViewDidLoad()
		{
			// Defaults are stored in ~/Library/Preferences/*.osu.macOS.Agent.plist
			if (defaults.ValueForKey((NSString) "location") != null)
				if (!SelectLocation(defaults.StringForKey("location")))
					return;

			if (LoadCheckbox(MapMoveCheckbox)) MapMoveCheckboxClick(null);
			if (LoadCheckbox(SkinMoveCheckbox)) SkinMoveCheckboxClick(null);
			if (LoadCheckbox(ReplayOpenCheckbox)) ReplayOpenCheckboxClick(null);
			if (LoadCheckbox(NotificationCheckbox)) NotificationsCheckboxClick(null);
		}

		private bool LoadCheckbox(NSButton box)
		{
			if (!defaults.BoolForKey(box.Title)) return false;
			box.State = NSCellStateValue.On;
			return true;
		}

		private bool SelectLocation(string location)
		{
			instance = new Instance(location);
			LocationTextField.StringValue = instance.rootPath;

			var version = instance.EngineVersion();
			EngineLabel.StringValue = version ?? "No Wineskin Detected";
			defaults.SetString(location, "location");
			SetControlsEnabled(version != null);
			return version != null;
		}

		private void SetControlsEnabled(bool enabled)
		{
			MapMoveCheckbox.Enabled = enabled;
			SkinMoveCheckbox.Enabled = enabled;
			ReplayOpenCheckbox.Enabled = enabled;
			OpenGameFolderButton.Enabled = enabled;
			UpdateButton.Enabled = enabled;

			ScanButton.Enabled = enabled;
			RepairButton.Enabled = false;
			ReportButton.Enabled = false;
		}

		private void RunPasses(Action<IPass, int> action)
		{
			PassProgress.StartAnimation(null);
			for (var index = 0; index < Pass.Passes.Length; ++index)
			{
				var pass = Pass.Passes[index];
				PassLabel.StringValue = pass.GetType().Name;
				NSRunLoop.Main.RunUntil(NSDate.Now);
				action(pass, index);
			}

			PassProgress.StopAnimation(null);
			PassLabel.StringValue = "";
		}
	}
}