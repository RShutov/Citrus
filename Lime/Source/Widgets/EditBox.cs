﻿using System;

namespace Lime
{
	public class EditBox : Frame
	{
		public SimpleText TextWidget { get; private set; }
		public Editor Editor { get; set; }
		public event Action<string> Submitted;
		public ScrollView Scroll { get; private set; }

		public override string Text
		{
			get { return TextWidget.Text; }
			set { TextWidget.Text = value; }
		}

		public EditBox()
		{
			Scroll = new ScrollView(this, ScrollDirection.Horizontal);
			Scroll.CanScroll = false;
			TextWidget = new SimpleText();
			TextWidget.Height = Height;
			Scroll.Content.AddNode(TextWidget);
			Theme.Current.Apply(this);
		}

		protected override void Awake()
		{
			TextWidget.Submitted += text => Submitted?.Invoke(text);
			base.Awake();
		}

		protected override void OnSizeChanged(Vector2 sizeDelta)
		{
			base.OnSizeChanged(sizeDelta);
			if (TextWidget != null) { // Size is assigned in Widget constructor.
				TextWidget.Height = Height;
				Editor?.AdjustSizeAndScrollToCaret();
			}
		}
	}
}

