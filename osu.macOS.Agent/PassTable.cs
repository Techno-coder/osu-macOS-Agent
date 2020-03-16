using System;
using System.Collections.Generic;
using AppKit;
using CoreGraphics;
using Foundation;

namespace osu.macOS.Agent
{
	public class PassTableDelegate : NSTableViewDelegate
	{
		private const string CellIdentifier = "Cell";

		private readonly PassTableSource source;

		public PassTableDelegate(PassTableSource source)
		{
			this.source = source;
		}

		public override NSView GetViewForItem(NSTableView tableView, NSTableColumn tableColumn, nint row)
		{
			var view = (NSTextField) tableView.MakeView(CellIdentifier, this) ?? new NSTextField
			{
				Identifier = CellIdentifier,
				BackgroundColor = NSColor.Clear,
				Bordered = false,
				Editable = false,
				Selectable = true
			};

			view.StringValue = "";
			view.Cell.Wraps = true;
			var entry = source.entries[(int) row];
			switch (tableColumn.Title)
			{
				case "Severity":
					if (entry.severity != Severity.None)
						view.StringValue = entry.severity.ToString();
					break;
				case "Status":
					if (entry.status != Status.None)
						view.StringValue = entry.status.ToString();
					break;
				case "Details":
					view.StringValue = entry.details;
					break;
			}

			return view;
		}

		public override nfloat GetRowHeight(NSTableView tableView, nint row)
		{
			var cell = new NSCell(source.entries[(int) row].details) {Wraps = true};
			var width = tableView.FindTableColumn((NSString) "Details").Width;
			var bounds = new CGRect(0, 0, width, nfloat.MaxValue);
			return cell.CellSizeForBounds(bounds).Height;
		}
	}

	public class PassTableSource : NSTableViewDataSource
	{
		public readonly List<Entry> entries = new List<Entry>();

		public override nint GetRowCount(NSTableView _)
		{
			return entries.Count;
		}
	}
}