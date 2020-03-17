using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using AppKit;
using Foundation;

namespace osu.macOS.Agent
{
	public partial class ViewController : NSViewController
	{
		private Instance instance;
		private FileSystemWatcher mapWatcher;
		private FileSystemWatcher skinWatcher;
		private PassTableSource passSource;

		private readonly NSUserDefaults defaults = NSUserDefaults.StandardUserDefaults;

		public ViewController(IntPtr handle) : base(handle) {}

		partial void SelectButton(NSObject sender)
		{
			var panel = new NSOpenPanel {Message = "Select osu!.app application"};
			panel.RunModal();

			if (panel.Filenames.Length == 0) return;
			SelectLocation(panel.Filenames[0]);
		}

		partial void MapMoveCheckboxClick(NSObject sender)
		{
			mapWatcher?.Dispose();
			if (!MapMoveCheckbox.Enabled || MapMoveCheckbox.State != NSCellStateValue.On) return;
			mapWatcher = WatchDownloads("*.osz", "Songs");
			defaults.SetBool(true, MapMoveCheckbox.Title);
		}

		partial void SkinMoveCheckboxClick(NSObject sender)
		{
			skinWatcher?.Dispose();
			if (!SkinMoveCheckbox.Enabled || SkinMoveCheckbox.State != NSCellStateValue.On) return;
			skinWatcher = WatchDownloads("*.osk", "Skins");
			defaults.SetBool(true, SkinMoveCheckbox.Title);
		}

		partial void NotificationsCheckboxClick(NSObject sender) =>
			defaults.SetBool(true, NotificationCheckbox.Title);

		partial void OpenGameFolderButtonClick(NSObject sender) =>
			NSWorkspace.SharedWorkspace.OpenFile(instance.DataPath());

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

		public override void ViewDidLoad()
		{
			// Defaults are stored in ~/Library/Preferences/*.osu.macOS.Agent.plist
			if (!SelectLocation(defaults.StringForKey("location"))) return;
			if (LoadCheckbox(MapMoveCheckbox)) MapMoveCheckboxClick(null);
			if (LoadCheckbox(SkinMoveCheckbox)) SkinMoveCheckboxClick(null);
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
			OpenGameFolderButton.Enabled = enabled;

			ScanButton.Enabled = enabled;
			RepairButton.Enabled = false;
			ReportButton.Enabled = false;
		}

		private FileSystemWatcher WatchDownloads(string pattern, string targetDirectory)
		{
			var source = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads");
			var watcher = new FileSystemWatcher(source, pattern) {NotifyFilter = NotifyFilters.FileName};
			watcher.Created += (_, arguments) =>
			{
				var target = Regex.Replace(arguments.Name, "[^a-zA-Z0-9-.\\s]", "");
				File.Move(arguments.FullPath, $"{instance.DataPath()}/{targetDirectory}/{target}");

				if (NotificationCheckbox.State == NSCellStateValue.Off) return;
				var notificationCenter = NSUserNotificationCenter.DefaultUserNotificationCenter;
				notificationCenter.DeliverNotification(new NSUserNotification
				{
					Title = "osu!macOS Agent",
					InformativeText = target
				});
			};

			watcher.EnableRaisingEvents = true;
			return watcher;
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