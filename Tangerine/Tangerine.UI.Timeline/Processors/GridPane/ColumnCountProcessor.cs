using System;
using System.Linq;
using Lime;
using Tangerine.Core;
using System.Collections.Generic;

namespace Tangerine.UI.Timeline
{
	public class ColumnCountProcessor : IProcessor
	{
		public IEnumerator<object> MainLoop()
		{
			var timeline = Timeline.Instance;
			while (true) {
				var rows = timeline.Rows;
				int maxColumn = 0;
				foreach (var row in rows) {
					var nodeData = row.Components.Get<Components.NodeRow>();
					if (nodeData != null) {
						maxColumn = Math.Max(maxColumn, nodeData.Node.Animators.GetOverallDuration());
					}
				}
				var maxVisibleColumn = ((timeline.ScrollOrigin.X + timeline.Grid.Size.X) / Metrics.TimelineColWidth).Ceiling();
				timeline.ColumnCount = Math.Max(maxColumn + 1, maxVisibleColumn);
				yield return null;
			}
		}
	}
}