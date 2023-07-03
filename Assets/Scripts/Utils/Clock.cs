using System;

using UnityEngine;

namespace SilgonGalaxy.Utils {
	[Serializable]
	public struct Clock {
		public float interval;
		public float Time {get; private set;}
		public readonly float NormalizedTime => Time / interval;
		
		public Action onTrigger;


		public Clock(float interval) {
			this.interval = interval;
			Time = 0;
			onTrigger = null;
		}

		public void Update(float deltaTime) {
			Time += deltaTime;
			while (Time >= interval && interval != 0) {
				Time -= interval;
				onTrigger?.Invoke();
			}
		}
	}
}