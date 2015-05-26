namespace Lime
{
	/// <summary>
	/// ������ ������ �������� ������ (������ ��������). �������� ��� ������� �����
	/// </summary>
	public sealed class World : Frame
	{
		/// <summary>
		/// ������, ������� � ������ ������ �������� ����� �����. ������ ��� ���������� � ������ Input.TextInput, ��������� ActiveTextWidget == this.
		/// ��� ������ ������ ����� � �������, �������� ActiveTextWidget
		/// </summary>
		public IKeyboardInputProcessor ActiveTextWidget;

		/// <summary>
		/// � ������ ����� ���������� ActiveTextWidget ������ ������������� ���� ���� � true
		/// </summary>
		public bool IsActiveTextWidgetUpdated;

		public static World Instance = new World();

#if iOS || ANDROID
		private IKeyboardInputProcessor prevActiveTextWidget;
#endif

		protected override void SelfUpdate(float delta)
		{
			WidgetInput.RemoveInvalidatedCaptures();
			ParticleEmitter.NumberOfUpdatedParticles = 0;
			IsActiveTextWidgetUpdated = false;
		}

		protected override void SelfLateUpdate(float delta)
		{
			if (!IsActiveTextWidgetUpdated) {
				ActiveTextWidget = null;
			}
#if iOS || ANDROID
			if (Application.IsMainThread) {
				bool showKeyboard = ActiveTextWidget != null && ActiveTextWidget.Visible;
				if (prevActiveTextWidget != ActiveTextWidget) {
					Application.Instance.SoftKeyboard.Show(showKeyboard, ActiveTextWidget != null ? ActiveTextWidget.Text : "");
				}
#if ANDROID
				if (!Application.Instance.SoftKeyboard.Visible) {
					ActiveTextWidget = null;
				}
#endif
				// Handle switching between various text widgets
				if (prevActiveTextWidget != ActiveTextWidget && ActiveTextWidget != null && prevActiveTextWidget != null) {
					Application.Instance.SoftKeyboard.ChangeText(ActiveTextWidget.Text);
				}

				prevActiveTextWidget = ActiveTextWidget;
			}
#endif
		}
	}
}
