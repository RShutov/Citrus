﻿#if !ANDROID && !iOS
using System;
using System.Collections.Generic;

namespace Lime
{
	public static class Theme
	{
		public static class Metrics
		{
			public static readonly int TextHeight = 16;
			public static readonly Vector2 CheckBoxSize = new Vector2(16, 16);
			public static readonly Vector2 DefaultButtonSize = new Vector2(75, 22);
			public static readonly Vector2 DefaultToolbarButtonSize = new Vector2(20, 20);
			public static readonly Vector2 DefaultEditBoxSize = new Vector2(75, 22);
			public static readonly Vector2 MaxTabSize = new Vector2(250, 24);
			public static readonly Vector2 MinTabSize = new Vector2(25, 24);
			public static readonly Vector2 CloseButtonSize = new Vector2(16, 16);
			public static readonly Thickness ControlsPadding = new Thickness(2);
		}

		public class ColorTheme
		{
			public Color4 BlackText;
			public Color4 GrayText;
			public Color4 WhiteBackground;
			public Color4 GrayBackground;
			public Color4 SelectedBackground;
			public Color4 HoveredBackground;
			public Color4 ControlBorder;
			public ColorGradient ButtonDefault;
			public ColorGradient ButtonHover;
			public ColorGradient ButtonPress;
			public ColorGradient ButtonDisable;
			public ColorGradient ExpandButtonDefault;
			public ColorGradient ExpandButtonHover;
			public ColorGradient ExpandButtonPress;
			public ColorGradient ExpandButtonDisable;
			public Color4 TabNormal;
			public Color4 TabActive;
			public Color4 SeparatorColor;
			public Color4 KeyboardFocusBorder;
			public Color4 TextSelection;
			public Color4 CloseButtonNormal;
			public Color4 CloseButtonHovered;
			public Color4 CloseButtonPressed;
			public Color4 ScrollbarBackground;
			public Color4 ScrollbarThumb;
			public Color4 TextCaret;
			public Color4 ZebraColor1;
			public Color4 ZebraColor2;
			public Color4 SelectedBorder;

			public static ColorTheme CreateDarkTheme()
			{
				var grayBackground = new Color4(45, 45, 48);
				var blackText = new Color4(204, 204, 204);
				return new ColorTheme {
					BlackText = blackText,
					GrayText = blackText.Darken(0.35f),
					WhiteBackground = Color4.Black.Lighten(0.25f),
					GrayBackground = new Color4(45, 45, 48),
					SelectedBackground = new Color4(86, 86, 86),
					SelectedBorder = new Color4(153, 209, 255),
					HoveredBackground = new Color4(86, 86, 86).Lighten(0.25f),
					ControlBorder = new Color4(39, 39, 39),
					ButtonDefault = new ColorGradient(new Color4(107, 107, 107), new Color4(107, 107, 107)),
					ButtonHover = new ColorGradient(new Color4(133, 133, 133), new Color4(133, 133, 133)),
					ButtonPress = new ColorGradient(new Color4(141, 167, 204), new Color4(141, 167, 204)),
					ButtonDisable = new ColorGradient(new Color4(64, 64, 64), new Color4(64, 64, 64)),
					ExpandButtonDefault = new ColorGradient(new Color4(107, 107, 107), new Color4(107, 107, 107)),
					ExpandButtonHover = new ColorGradient(new Color4(133, 133, 133), new Color4(133, 133, 133)),
					ExpandButtonPress = new ColorGradient(new Color4(141, 167, 204), new Color4(141, 167, 204)),
					ExpandButtonDisable = new ColorGradient(new Color4(64, 64, 64), new Color4(64, 64, 64)),
					TabNormal = grayBackground.Lighten(0.1f),
					TabActive = grayBackground.Lighten(0.05f),
					SeparatorColor = grayBackground.Lighten(0.3f),
					KeyboardFocusBorder = new Color4(100, 150, 200),
					TextSelection = new Color4(133, 133, 133),
					CloseButtonNormal = grayBackground.Lighten(0.6f),
					CloseButtonHovered = grayBackground.Lighten(0.8f),
					CloseButtonPressed = grayBackground.Lighten(1),
					ScrollbarBackground = new Color4(51, 51, 51),
					ScrollbarThumb = new Color4(107, 107, 107),
					TextCaret = new Color4(204, 204, 204),
					ZebraColor1 = grayBackground.Lighten(0.2f),
					ZebraColor2 = grayBackground.Lighten(0.3f),
				};

			}

			public static ColorTheme CreateLightTheme()
			{
				var grayBackground = new Color4(240, 240, 240);
				return new ColorTheme {
					BlackText = Color4.Black,
					GrayText = Color4.Black.Lighten(0.35f),
					WhiteBackground = Color4.White,
					GrayBackground = grayBackground,
					SelectedBackground = new Color4(205, 232, 255),
					SelectedBorder = new Color4(153, 209, 255),
					HoveredBackground = new Color4(229, 243, 255),
					ControlBorder = new Color4(172, 172, 172).Lighten(0.3f),
					ButtonDefault = new ColorGradient(new Color4(239, 239, 239), new Color4(229, 229, 229)),
					ButtonHover = new ColorGradient(new Color4(235, 244, 252), new Color4(222, 238, 252)),
					ButtonPress = new ColorGradient(new Color4(215, 234, 252), new Color4(199, 226, 252)),
					ButtonDisable = new ColorGradient(new Color4(244, 244, 244), new Color4(244, 244, 244)),
					ExpandButtonDefault = new ColorGradient(new Color4(107, 107, 107), new Color4(107, 107, 107)),
					ExpandButtonHover = new ColorGradient(new Color4(133, 133, 133), new Color4(133, 133, 133)),
					ExpandButtonPress = new ColorGradient(new Color4(141, 167, 204), new Color4(141, 167, 204)),
					ExpandButtonDisable = new ColorGradient(new Color4(64, 64, 64), new Color4(64, 64, 64)),
					TabNormal = grayBackground.Darken(0.1f),
					TabActive = grayBackground.Darken(0.05f),
					SeparatorColor = grayBackground.Darken(0.3f),
					KeyboardFocusBorder = new Color4(0, 120, 215),
					TextSelection = new Color4(200, 230, 255),
					CloseButtonNormal = grayBackground.Darken(0.6f),
					CloseButtonHovered = grayBackground.Darken(0.8f),
					CloseButtonPressed = grayBackground.Darken(1),
					ScrollbarBackground = new Color4(210, 210, 210),
					ScrollbarThumb = new Color4(120, 120, 120),
					TextCaret = Color4.Black,
					ZebraColor1 = Color4.White,
					ZebraColor2 = Color4.White.Darken(0.1f),
				};
			}
		}

		public static ColorTheme Colors = ColorTheme.CreateLightTheme();

		public class KeyboardFocusBorderPresenter : CustomPresenter
		{
			private float thikness = 1.0f;
			public KeyboardFocusBorderPresenter(float thikness = 1.0f)
			{
				this.thikness = thikness;
			}
			public override void Render(Node node)
			{
				if (Widget.Focused == node && !node.AsWidget.IsMouseOverThisOrDescendant()) {
					var widget = node.AsWidget;
					widget.PrepareRendererState();
					Renderer.DrawRectOutline(Vector2.Zero, widget.Size, Colors.KeyboardFocusBorder, thikness);
				}
			}
		}

		internal class MouseHoverBorderPresenter : CustomPresenter
		{
			public override void Render(Node node)
			{
				var widget = node.AsWidget;
				if (widget.IsMouseOverThisOrDescendant()) {
					widget.PrepareRendererState();
					Renderer.DrawRectOutline(Vector2.Zero, widget.Size, Colors.KeyboardFocusBorder, 1);
				}
			}
		}

		internal static IEnumerator<object> MouseHoverInvalidationTask(Widget widget)
		{
			var isHovered = widget.IsMouseOverThisOrDescendant();
			while (true) {
				var isMouseOver = widget.IsMouseOverThisOrDescendant();
				if (isMouseOver != isHovered) {
					isHovered = isMouseOver;
					Window.Current.Invalidate();
				}
				yield return null;
			}
		}
	}
}
#endif
