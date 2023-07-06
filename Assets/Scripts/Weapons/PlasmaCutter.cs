using System;
using UnityEngine;

namespace SilgonGalaxy.Weapons {
	using Utils;
	using Extensions;

	[Serializable]
	public sealed class PlasmaCutter {
		public Config config;
		public References references;
		public Clock flashClock;

		private float heat;
		private bool isFiring;
		private bool isOverheated;
		private bool CanFire => !isOverheated;


		public void Update(float dt) {
			isFiring = isFiring && CanFire;
			references.beam.enabled = isFiring;
			
			var (heatTarget, heatTime) = isFiring
				? (1, config.heatBuildupTime)
				: (0, config.heatShedTime);
			
			heat = Mathf.MoveTowards(heat, heatTarget, dt / heatTime);

			if (heat >= 1) Overheat();
			isOverheated = isOverheated && !(heat <= 0);
			
			var normalizedBeamWidth =
				(flashClock.NormalizedTime * MathExtensions.TAU).Sin() * 0.5f + 0.5f;

			var beamWidth =
				Mathf.Lerp(
					Mathf.Lerp(
						config.minCoolBeamWidth,
						config.maxCoolBeamWidth,
						normalizedBeamWidth
					),
					Mathf.Lerp(
						config.minHotBeamWidth,
						config.maxHotBeamWidth,
						normalizedBeamWidth
					),
					heat
				);

			flashClock.Update(dt);
			references.beam.transform.localScale = new Vector3(
				beamWidth,
				1,
				1
			);
		}

		public void StartFire() {
			if (CanFire) {
				isFiring = true;
				references.audioSource.PlayOneShot(config.startupSound);
				references.audioSource.clip = config.loopSound;
				references.audioSource.loop = true;
				references.audioSource.PlayScheduled(config.startupSound.length + AudioSettings.dspTime);
			}
			else
				references.audioSource.PlayOneShot(config.startFailSound);
		}
		
		public void ReleaseFire() {
			if (!isFiring) return;
			references.audioSource.loop = false;
			references.audioSource.Stop();
			references.audioSource.PlayOneShot(config.shutdownSound);
			isFiring = false;
		}

		private void Overheat() {
			isOverheated = true;
			references.audioSource.loop = false;
			references.audioSource.Stop();
			references.audioSource.PlayOneShot(config.overheatSound);
		}

		[Serializable]
		public struct Config {
			public float heatBuildupTime;
			public float heatShedTime;
			public float minCoolBeamWidth;
			public float maxCoolBeamWidth;
			public float minHotBeamWidth;
			public float maxHotBeamWidth;

			public AudioClip startupSound;
			public AudioClip loopSound;
			public AudioClip shutdownSound;
			public AudioClip overheatSound;
			public AudioClip startFailSound;
		}

		[Serializable]
		public struct References {
			public SpriteRenderer beam;
			public AudioSource audioSource;
		}
	}
}