using System;
using System.Collections.Generic;
using System.Linq;
using Lime;
using Tangerine.Core;

namespace Tangerine.UI.Timeline.Operations
{
	public class ClearGridSelection : IOperation
	{
		GridSelection savedSelection;

		public void Do()
		{
			savedSelection = Timeline.Instance.GridSelection;
			Timeline.Instance.GridSelection = new GridSelection();
		}

		public void Undo()
		{
			Timeline.Instance.GridSelection = savedSelection;
			savedSelection = null;
		}
	}
}