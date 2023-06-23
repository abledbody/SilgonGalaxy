using UnityEngine;

namespace SilgonGalaxy.Weapons {
	[System.Serializable]
	public sealed class ChargeBlaster {
		public Config config;

		private float charge;
		private bool firing;


		public void Update(float dt) {
			if (firing && charge < config.chargeTime)
				charge += dt;
		}

		public void StartFire() {
			charge = 0;
			firing = true;
		}
		
		public void ReleaseFire() {
			if (!firing) return;

			firing = false;

			if (charge >= config.chargeTime) {
				Debug.Log($"Big shooty");
			}
			else {
				Debug.Log($"Small shooty");
			}
		}

		[System.Serializable]
		public struct Config {
			[Min(float.Epsilon)]
			public float chargeTime;
		}
	}
}