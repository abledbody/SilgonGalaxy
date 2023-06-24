using System;

namespace SilgonGalaxy.Utils {
	[Serializable]
	public struct Delay {
		public Action onTriggered;
		public Action onComplete;
		public Action onCancel;

		public float duration;
		public float clock;
		public bool Complete => clock <= 0;


		public Delay(float duration) {
			onTriggered = null;
			onComplete = null;
			onCancel = null;

			this.duration = duration;
			clock = 0;
		}

		public bool Update(float dt) {
			bool wasIncomplete = !Complete;

			if (wasIncomplete)
				clock -= dt;
			
			if (Complete && wasIncomplete) {
				onComplete?.Invoke();
				return true;
			}

			return false;
		}

		public void Start(float? duration = null, bool set = true) {
			clock = duration ?? this.duration;
			if (set)
				this.duration = duration ?? this.duration;
			onTriggered?.Invoke();
		}

		public void Cancel() {
			clock = 0;
			onCancel?.Invoke();
		}
	}
}