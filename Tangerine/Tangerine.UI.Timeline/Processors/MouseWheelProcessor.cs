﻿using System;
using System.Collections.Generic;
using Lime;
using Tangerine.UI.Timeline.Operations;

namespace Tangerine.UI.Timeline
{
	public class MouseWheelProcessor : Core.ITaskProvider
	{
		Timeline timeline => Timeline.Instance;

		public IEnumerator<object> Task()
		{
			var rollHandler = HandleScroll(timeline.Roll.RootWidget.Input);
			var gridHandler = HandleScroll(timeline.Grid.RootWidget.Input);
			var overviewHandler = HandleScroll(timeline.Overview.RootWidget.Input);
			while (true) {
				rollHandler.MoveNext();
				gridHandler.MoveNext();
				overviewHandler.MoveNext();
				HandleCellsMagnification(timeline.Grid.RootWidget.Input);
				yield return null;
			}
		}

		private IEnumerator<object> HandleScroll(WidgetInput input)
		{
			var prevPosition = Vector2.Zero;
			while (true) {
				var scrollDelta = GetWheelDelta(input);
				if (scrollDelta != 0 && !input.IsKeyPressed(Key.Alt)) {
					timeline.OffsetY += scrollDelta * TimelineMetrics.DefaultRowHeight;
				}
				if (input.IsKeyPressed(Key.Mouse2)) {
					var delta = input.MousePosition - prevPosition;
					if (delta != Vector2.Zero && (timeline.Offset.X - delta.X > 0 || timeline.Offset.Y - delta.Y > 0)) {
						timeline.Offset -= delta;
						Core.Operations.Dummy.Perform();
					}
				}
				prevPosition = input.MousePosition;
				yield return null;
			}
		}

		void HandleCellsMagnification(WidgetInput input)
		{
			var delta = GetWheelDelta(input);
			if (delta != 0 && input.IsKeyPressed(Key.Alt)) {
				var prevColWidth = TimelineMetrics.ColWidth;
				TimelineMetrics.ColWidth = (TimelineMetrics.ColWidth + delta).Clamp(5, 30);
				if (prevColWidth != TimelineMetrics.ColWidth) {
					var mp = timeline.Grid.RootWidget.Input.LocalMousePosition.X + Timeline.Instance.Offset.X;
					Timeline.Instance.OffsetX += (mp / prevColWidth) * delta;
					Core.Operations.Dummy.Perform();
				}
			}
		}

		int GetWheelDelta(WidgetInput input)
		{
			if (input.WasKeyPressed(Key.MouseWheelDown)) {
				return 1;
			}
			if (input.WasKeyPressed(Key.MouseWheelUp)) {
				return -1;
			}
			return 0;
		}
	}
}

