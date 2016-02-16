namespace Lime
{
	public struct Thickness
	{
		public float Left;
		public float Top;
		public float Right;
		public float Bottom;

		public Thickness(float overall)
		{
			Left = Top = Right = Bottom = overall;
		}

		public Thickness(float horizontal, float vertical)
		{
			Left = Right = horizontal;
			Top = Bottom = vertical;
		}

		public static Vector2 operator + (Vector2 size, Thickness padding)
		{
			return new Vector2(
				size.X + padding.Left + padding.Right,
				size.Y + padding.Top + padding.Bottom);
		}

		public static Vector2 operator - (Vector2 size, Thickness padding)
		{
			return new Vector2(
				size.X - padding.Left - padding.Right,
				size.Y - padding.Top - padding.Bottom);
		}
	}
}