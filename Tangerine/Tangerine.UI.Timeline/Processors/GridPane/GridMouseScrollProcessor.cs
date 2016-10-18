using System;
using System.Linq;
using System.Collections.Generic;
using Lime;

namespace Tangerine.UI.Timeline
{	
	class GridMouseScrollProcessor : Core.IProcessor
	{
		Timeline timeline => Timeline.Instance;

		public IEnumerator<object> Loop()
		{
			var input = timeline.Grid.RootWidget.Input;
			while (true) {
				if (input.IsMouseOwner()) {
					var rect = timeline.Grid.RootWidget.CalcAABBInSpaceOf(timeline.PanelWidget);
					if (input.MousePosition.X > rect.B.X) {
						timeline.ScrollPos.X += TimelineMetrics.ColWidth;
					} else if (input.MousePosition.X < rect.A.X) {
						timeline.ScrollPos.X -= TimelineMetrics.ColWidth;
					} else if (input.MousePosition.Y > rect.B.Y) {
						timeline.ScrollPos.Y += TimelineMetrics.DefaultRowHeight;
					} else if (input.MousePosition.Y < rect.A.Y) {
						timeline.ScrollPos.Y -= TimelineMetrics.DefaultRowHeight;
					}
					Window.Current.Invalidate();
				}
				yield return null;
			}
		}
	}	
}
