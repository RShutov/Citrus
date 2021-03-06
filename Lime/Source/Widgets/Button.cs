using System;
using System.Linq;
using System.Collections.Generic;
using Yuzu;

namespace Lime
{
	[AllowedChildrenTypes(typeof(Node))]
	public class Button : Widget
	{
		private TextPresentersFeeder textPresentersFeeder;
		private IEnumerator<int> stateHandler;
		private ClickGesture clickGesture;
		private bool isChangingState;
		private bool isDisabledState;

		public BitSet32 EnableMask = BitSet32.Full;

		[YuzuMember]
		public override string Text { get; set; }

		[YuzuMember]
		public bool Enabled
		{
			get { return EnableMask[0]; }
			set { EnableMask[0] = value; }
		}

		public override Action Clicked { get; set; }

		public Button()
		{
			HitTestTarget = true;
			Input.AcceptMouseBeyondWidget = false;
		}

		private void SetState(IEnumerator<int> newState)
		{
			stateHandler = newState;
			if (!isChangingState) {
				isChangingState = true;
				stateHandler.MoveNext();
				isChangingState = false;
			}
		}

		private IEnumerator<int> InitialState()
		{
			yield return 0;
			SetState(NormalState());
		}

		public override bool WasClicked() => clickGesture?.WasRecognized() ?? false;

		protected override void Awake()
		{
			// On the current frame the button contents may not be loaded,
			// so delay its initialization until the next frame.
			SetState(InitialState());
			textPresentersFeeder = new TextPresentersFeeder(this);
			clickGesture = new ClickGesture();
			Gestures.Add(clickGesture);
		}

		private IEnumerator<int> NormalState()
		{
			TryRunAnimation("Normal");
			while (true) {
#if WIN || MAC
				if (IsMouseOverThisOrDescendant()) {
					SetState(HoveredState());
				}
#else
				if (clickGesture.WasBegan()) {
					SetState(PressedState());
				}
#endif
				yield return 0;
			}
		}

		private IEnumerator<int> HoveredState()
		{
			TryRunAnimation("Focus");
			while (true) {
				if (!IsMouseOverThisOrDescendant()) {
					SetState(NormalState());
				} else if (clickGesture.WasBegan()) {
					SetState(PressedState());
				}
				yield return 0;
			}
		}

		private IEnumerator<int> PressedState()
		{
			TryRunAnimation("Press");
			var wasMouseOver = true;
			while (true) {
				if (clickGesture.WasRecognized()) {
					while (IsRunning) {
						yield return 0;
					}
					Clicked?.Invoke();
					// buz: don't play release animation
					// if button's parent became invisible due to
					// button press (or it will be played when
					// parent is visible again)
					if (!GloballyVisible) {
#if WIN || MAC
						SetState(HoveredState());
#else
						SetState(NormalState());
#endif
					} else {
						SetState(ReleaseState());
					}
				} else if (clickGesture.WasCanceled()) {
					if (CurrentAnimation == "Press") {
						TryRunAnimation("Release");
						while (IsRunning) {
							yield return 0;
						}
					}
					SetState(NormalState());
				}
				var mouseOver = IsMouseOverThisOrDescendant();
				if (wasMouseOver && !mouseOver) {
					TryRunAnimation("Release");
				} else if (!wasMouseOver && mouseOver) {
					TryRunAnimation("Press");
				}
				wasMouseOver = mouseOver;
				yield return 0;
			}
		}

		private IEnumerator<int> ReleaseState()
		{
			if (CurrentAnimation != "Release") {
				if (TryRunAnimation("Release")) {
					while (IsRunning) {
						yield return 0;
					}
				}
			}
#if WIN || MAC
			SetState(HoveredState());
#else
			SetState(NormalState());
#endif
		}

		private IEnumerator<int> DisabledState()
		{
			isDisabledState = true;
			if (CurrentAnimation == "Release") {
				// The release animation should be played if we disable the button
				// right after click on it.
				while (IsRunning) {
					yield return 0;
				}
			}
			TryRunAnimation("Disable");
			while (IsRunning) {
				yield return 0;
			}
			while (!EnableMask.All()) {
				yield return 0;
			}
			TryRunAnimation("Enable");
			while (IsRunning) {
				yield return 0;
			}
			isDisabledState = false;
			SetState(NormalState());
		}

		public override void Update(float delta)
		{
			base.Update(delta);
			if (GloballyVisible) {
				stateHandler.MoveNext();
				textPresentersFeeder.Update();
			}
			if (!EnableMask.All() && !isDisabledState) {
				SetState(DisabledState());
			}
#if WIN || MAC
			if (Enabled) {
				if (Input.ConsumeKeyPress(Key.Space) || Input.ConsumeKeyPress(Key.Enter)) {
					Clicked?.Invoke();
				}
			}
#endif
		}
	}

	internal class TextPresentersFeeder
	{
		private Widget widget;
		private List<Widget> textPresenters;

		public TextPresentersFeeder(Widget widget)
		{
			this.widget = widget;
		}

		public void Update()
		{
			textPresenters = textPresenters ?? widget.Descendants.OfType<Widget>().Where(i => i.Id == "TextPresenter").ToList();
			foreach (var i in textPresenters) {
				i.Text = widget.Text;
			}
		}
	}
}
