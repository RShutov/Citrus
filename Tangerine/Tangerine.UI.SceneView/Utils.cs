﻿using System;
using System.Linq;
using System.Collections.Generic;
using Lime;

namespace Tangerine.UI.SceneView
{
	public static class Utils
	{
		public static void ChangeCursorIfDefault(MouseCursor cursor)
		{
			if (WidgetContext.Current.MouseCursor == MouseCursor.Default) {
				WidgetContext.Current.MouseCursor = cursor;
			}
		}

		public static Vector2 Snap(this Vector2 value, Vector2 origin, float distanceTolerance = 0.001f)
		{
			return (value - origin).Length > distanceTolerance ? value : origin;
		}

		public static float Snap(this float value, float origin, float distanceTolerance = 0.001f)
		{
			return (value - origin).Abs() > distanceTolerance ? value : origin;
		}

		public static IEnumerable<Widget> UnlockedWidgets()
		{
			return Core.Document.Current.SelectedNodes().OfType<Widget>().Where(w => !w.GetTangerineFlag(TangerineFlags.Locked));
		}

		public static bool CalcHullAndPivot(IEnumerable<Widget> widgets, Widget canvas, out Quadrangle hull, out Vector2 pivot)
		{
			Widget first = null;
			var pivotsEqual = true;
			var aabb = Rectangle.Empty;
			pivot = Vector2.Zero;
			hull = new Quadrangle();
			foreach (var widget in widgets) {
				if (first == null) {
					hull = widget.CalcHullInSpaceOf(canvas);
					pivot = widget.CalcPositionInSpaceOf(canvas);
					aabb = widget.CalcAABBInSpaceOf(canvas);
					first = widget;
				} else {
					var t = widget.CalcAABBInSpaceOf(canvas);
					aabb = aabb
						.IncludingPoint(t.A)
						.IncludingPoint(new Vector2(t.Right, t.Top))
						.IncludingPoint(t.B)
						.IncludingPoint(new Vector2(t.Left, t.Bottom));
					hull = aabb.ToQuadrangle();
					pivotsEqual &= widget.CalcPositionInSpaceOf(canvas) == pivot;
				}
			}
			if (first == null) {
				return false;
			}
			if (!pivotsEqual) {
				pivot = aabb.Center;
			}
			return true;
		}
	}
}
