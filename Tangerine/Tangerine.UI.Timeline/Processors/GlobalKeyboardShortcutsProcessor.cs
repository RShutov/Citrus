using System;
using System.Linq;
using Lime;
using Tangerine.Core;
using System.Collections.Generic;

namespace Tangerine.UI.Timeline
{
	public class GlobalKeyboardShortcutsProcessor : Core.IProcessor
	{
		readonly WidgetInput input;

		Timeline timeline => Timeline.Instance;

		public GlobalKeyboardShortcutsProcessor(WidgetInput input)
		{
			this.input = input;
		}

		public IEnumerator<object> Loop()
		{
			while (true) {
				if (Document.Current != null) {
					HandleShortcuts(input);
					HorizontalScroll(input);
					VerticalScroll(input);
					HandleEnterExit(input);
				}
				yield return Task.WaitForInput();
			}
		}

		void HandleShortcuts(WidgetInput input)
		{
			var hasSelection = Document.Current.SelectedRows.Count > 0;
			input.EnableKey(Key.Commands.Undo, Document.Current.History.UndoEnabled);
			input.EnableKey(Key.Commands.Redo, Document.Current.History.RedoEnabled);
			input.EnableKey(Key.Commands.SelectAll, hasSelection);
			input.EnableKey(Key.Commands.Cut, hasSelection);
			input.EnableKey(Key.Commands.Copy, hasSelection);
			input.EnableKey(Key.Commands.Paste, Core.Operations.Clipboard.Nodes.Count > 0);
			input.EnableKey(Key.Commands.Delete, hasSelection);
			if (input.ConsumeKeyRepeat(Key.Commands.Redo)) {
				Document.Current.History.Redo();
			} else if (input.ConsumeKeyRepeat(Key.Commands.Undo)) {
				Document.Current.History.Undo();
			} else if (input.ConsumeKeyRepeat(Key.Commands.SelectAll)) {
				foreach (var row in Document.Current.Rows) {
					Core.Operations.SelectRow.Perform(row, true);
				}
			} else if (input.ConsumeKeyRepeat(Key.Commands.Cut)) {
				Core.Operations.Cut.Perform();
			} else if (input.ConsumeKeyRepeat(Key.Commands.Copy)) {
				Core.Operations.Copy.Perform();
			} else if (input.ConsumeKeyRepeat(Key.Commands.Paste)) {
				Core.Operations.Paste.Perform();
			} else if (input.ConsumeKeyRepeat(Key.Commands.Delete)) {
				Core.Operations.Delete.Perform();
			}
		}

		void HandleEnterExit(WidgetInput input)
		{
			var doc = Document.Current;
			if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.EnterNode)) {
				var node = Document.Current.EnumerateSelectedNodes().FirstOrDefault();
				if (node != null) {
					Core.Operations.EnterNode.Perform(node);
				}
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.ExitNode)) {
				Core.Operations.LeaveNode.Perform();
			}
		}
		
		void VerticalScroll(WidgetInput input)
		{
			if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.ScrollUp)) {
				SelectRow(-1, false);
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.ScrollDown)) {
				SelectRow(1, false);
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.SelectUp)) {
				SelectRow(-1, true);
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.SelectDown)) {
				SelectRow(1, true);
			}
		}

		void SelectRow(int advance, bool multiselection)
		{
			var doc = Document.Current;
			if (doc.Rows.Count == 0) {
				return;
			}
			var lastSelectedRow = doc.SelectedRows.Count > 0 ? doc.SelectedRows[0] : doc.Rows[0];
			var nextRow = doc.Rows[Mathf.Clamp(lastSelectedRow.Index + advance, 0, doc.Rows.Count - 1)];
			if (nextRow != lastSelectedRow) {
				if (!multiselection) {
					Core.Operations.ClearRowSelection.Perform();
				}
				if (doc.SelectedRows.Contains(nextRow)) {
					Core.Operations.SelectRow.Perform(lastSelectedRow, false);
				}
				Core.Operations.SelectRow.Perform(nextRow);
				timeline.EnsureRowVisible(nextRow);
			}
		}

		void HorizontalScroll(WidgetInput input)
		{
			int stride = 0;
			if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.FastScrollLeft)) {
				stride = -10;
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.FastScrollRight)) {
				stride = 10;
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.ScrollRight)) {
				stride = 1;
			} else if (input.ConsumeKeyRepeat(KeyBindings.TimelineKeys.ScrollLeft)) {
				stride = -1;
			}
			if (stride != 0) {
				Operations.SetCurrentColumn.Perform(Math.Max(0, timeline.CurrentColumn + stride));
			}
		}		
	}	
}