using System;
using System.IO;
using System.IO.Compression;
using System.Text.RegularExpressions;
using AppKit;
using Foundation;

namespace osu.macOS.Agent
{
	public partial class ViewController : NSViewController
	{
		private FileSystemWatcher mapWatcher;
		private FileSystemWatcher skinWatcher;
		private FileSystemWatcher replayWatcher;

		partial void SelectButton(NSObject sender)
		{
			var panel = new NSOpenPanel
			{
				Message = "Select osu!.app application",
				AllowedFileTypes = new[] {"app"}
			};

			panel.RunModal();
			if (panel.Filenames.Length == 0) return;
			SelectLocation(panel.Filenames[0]);
		}

		partial void MapMoveCheckboxClick(NSObject sender)
		{
			mapWatcher?.Dispose();
			if (!MapMoveCheckbox.Enabled || MapMoveCheckbox.State != NSCellStateValue.On) return;
			defaults.SetBool(true, MapMoveCheckbox.Title);
			mapWatcher = WatchDownloads("*.osz", (path, target) =>
				File.Move(path, $"{instance.DataPath()}/Songs/{target}"));
		}

		partial void SkinMoveCheckboxClick(NSObject sender)
		{
			skinWatcher?.Dispose();
			if (!SkinMoveCheckbox.Enabled || SkinMoveCheckbox.State != NSCellStateValue.On) return;
			defaults.SetBool(true, SkinMoveCheckbox.Title);
			skinWatcher = WatchDownloads("*.osk", (path, target) =>
			{
				target = Path.GetFileNameWithoutExtension(target);
				target = $"{instance.DataPath()}/Skins/{target}";
				ZipFile.ExtractToDirectory(path, target);
				File.Delete(path);
			});
		}

		partial void ReplayOpenCheckboxClick(NSObject sender)
		{
			replayWatcher?.Dispose();
			if (!ReplayOpenCheckbox.Enabled || ReplayOpenCheckbox.State != NSCellStateValue.On) return;
			defaults.SetBool(true, ReplayOpenCheckbox.Title);
			replayWatcher = WatchDownloads("*.osr", (path, target) =>
			{
				target = Regex.Replace(target, @"\s+", "");
				target = $"{instance.DataPath()}/Downloads/{target}";
				var command = $"open -a '{instance.rootPath}' '{target}'";

				File.Move(path, target);
				Instance.ExecuteCommand(command);
			});
		}

		partial void NotificationsCheckboxClick(NSObject sender) =>
			defaults.SetBool(true, NotificationCheckbox.Title);

		partial void OpenGameFolderButtonClick(NSObject sender) =>
			NSWorkspace.SharedWorkspace.OpenFile(instance.DataPath());

		private FileSystemWatcher WatchDownloads(string pattern, Action<string, string> action)
		{
			var source = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Downloads");
			var watcher = new FileSystemWatcher(source, pattern) {NotifyFilter = NotifyFilters.FileName};
			watcher.Created += (_, arguments) =>
			{
				var target = Regex.Replace(arguments.Name, "[^a-zA-Z0-9-.\\s]", "");
				action(arguments.FullPath, target);

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
	}
}