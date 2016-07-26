using System;
using System.Collections.Generic;
using System.Linq;
using Lime;
using Tangerine.Core;

namespace Tangerine.UI.Timeline.Operations
{
	public class SetCurrentColumn : IOperation
	{
		readonly int previousColumn;
		int currentColumn;

		public bool IsChangingDocument => false;
		public DateTime Timestamp { get; set; }

		public static void Perform(int currentColumn)
		{
			Document.Current.History.Perform(new SetCurrentColumn(currentColumn));
		}

		private SetCurrentColumn(int currentColumn)
		{
			previousColumn = Timeline.Instance.CurrentColumn;
			this.currentColumn = currentColumn;
		}

		public void Do()
		{
			Timeline.Instance.CurrentColumn = currentColumn;
			Timeline.Instance.EnsureColumnVisible(currentColumn);
		}

		public void Undo()
		{
			Timeline.Instance.CurrentColumn = previousColumn;
			Timeline.Instance.EnsureColumnVisible(previousColumn);
		}
	}
}