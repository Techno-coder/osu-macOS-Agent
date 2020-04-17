// WARNING
//
// This file has been generated automatically by Rider IDE
//   to store outlets and actions made in Xcode.
// If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using Foundation;
using System.CodeDom.Compiler;

namespace osu.macOS.Agent
{
	[Register ("ViewController")]
	partial class ViewController
	{
		[Outlet]
		AppKit.NSTextField EngineLabel { get; set; }

		[Outlet]
		AppKit.NSTextField LocationTextField { get; set; }

		[Outlet]
		AppKit.NSButton MapMoveCheckbox { get; set; }

		[Outlet]
		AppKit.NSButton NotificationCheckbox { get; set; }

		[Outlet]
		AppKit.NSButton OpenGameFolderButton { get; set; }

		[Outlet]
		AppKit.NSTextField PassLabel { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator PassProgress { get; set; }

		[Outlet]
		AppKit.NSTableView PassTable { get; set; }

		[Outlet]
		AppKit.NSButtonCell RepairButton { get; set; }

		[Outlet]
		AppKit.NSButtonCell ReportButton { get; set; }

		[Outlet]
		AppKit.NSButtonCell ScanButton { get; set; }

		[Outlet]
		AppKit.NSButton SkinMoveCheckbox { get; set; }

		[Outlet]
		AppKit.NSButton UpdateButton { get; set; }

		[Outlet]
		AppKit.NSProgressIndicator UpdateProgress { get; set; }

		[Action ("MapMoveCheckboxClick:")]
		partial void MapMoveCheckboxClick (Foundation.NSObject sender);

		[Action ("NotificationsCheckboxClick:")]
		partial void NotificationsCheckboxClick (Foundation.NSObject sender);

		[Action ("OpenGameFolderButtonClick:")]
		partial void OpenGameFolderButtonClick (Foundation.NSObject sender);

		[Action ("RepairButtonClick:")]
		partial void RepairButtonClick (Foundation.NSObject sender);

		[Action ("ReportButtonClick:")]
		partial void ReportButtonClick (Foundation.NSObject sender);

		[Action ("ScanButtonClick:")]
		partial void ScanButtonClick (Foundation.NSObject sender);

		[Action ("SelectButton:")]
		partial void SelectButton (Foundation.NSObject sender);

		[Action ("SkinMoveCheckboxClick:")]
		partial void SkinMoveCheckboxClick (Foundation.NSObject sender);

		[Action ("UpdateButtonClick:")]
		partial void UpdateButtonClick (Foundation.NSObject sender);

		void ReleaseDesignerOutlets ()
		{
			if (EngineLabel != null) {
				EngineLabel.Dispose ();
				EngineLabel = null;
			}

			if (LocationTextField != null) {
				LocationTextField.Dispose ();
				LocationTextField = null;
			}

			if (MapMoveCheckbox != null) {
				MapMoveCheckbox.Dispose ();
				MapMoveCheckbox = null;
			}

			if (NotificationCheckbox != null) {
				NotificationCheckbox.Dispose ();
				NotificationCheckbox = null;
			}

			if (OpenGameFolderButton != null) {
				OpenGameFolderButton.Dispose ();
				OpenGameFolderButton = null;
			}

			if (PassLabel != null) {
				PassLabel.Dispose ();
				PassLabel = null;
			}

			if (PassProgress != null) {
				PassProgress.Dispose ();
				PassProgress = null;
			}

			if (PassTable != null) {
				PassTable.Dispose ();
				PassTable = null;
			}

			if (RepairButton != null) {
				RepairButton.Dispose ();
				RepairButton = null;
			}

			if (ReportButton != null) {
				ReportButton.Dispose ();
				ReportButton = null;
			}

			if (ScanButton != null) {
				ScanButton.Dispose ();
				ScanButton = null;
			}

			if (SkinMoveCheckbox != null) {
				SkinMoveCheckbox.Dispose ();
				SkinMoveCheckbox = null;
			}

			if (UpdateButton != null) {
				UpdateButton.Dispose ();
				UpdateButton = null;
			}

			if (UpdateProgress != null) {
				UpdateProgress.Dispose ();
				UpdateProgress = null;
			}

		}
	}
}
