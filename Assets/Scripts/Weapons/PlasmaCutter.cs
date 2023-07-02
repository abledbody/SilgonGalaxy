using System;
using UnityEngine;

namespace SilgonGalaxy.Weapons {
	using Utils;

	[Serializable]
	public sealed class PlasmaCutter {
		public Config config;
		public References references;

		private float heat;
		private bool isFiring;
		private bool isOverheated;


		public void Update(float dt) {
			isFiring = isFiring && !isOverheated;
			
			var (heatTarget, heatTime) = isFiring
				? (1, config.heatBuildupTime)
				: (0, config.heatShedTime);
			
			heat = Mathf.MoveTowards(heat, heatTarget, dt / heatTime);

			isOverheated =
				isOverheated
				&& !(heat <= 0)
				|| heat >= 1;
		}

		public void StartFire() {
			isFiring = true;
		}
		
		public void ReleaseFire() {
			isFiring = false;
		}

		[Serializable]
		public struct Config {
			public float heatBuildupTime;
			public float heatShedTime;
		}

		[Serializable]
		public struct References {
			public Transform spawnPosition;
			public AudioSource audioSource;
		}
	}
}