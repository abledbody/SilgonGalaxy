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
		private bool fireInput;
		private bool isFiring;
		private bool isOverheated;


		public void Update(float dt) {
			isFiring = fireInput && !isOverheated;
			references.beam.enabled = isFiring;
			
			var (heatTarget, heatTime) = isFiring
				? (1, config.heatBuildupTime)
				: (0, config.heatShedTime);
			
			heat = Mathf.MoveTowards(heat, heatTarget, dt / heatTime);

			var wasOverheated = isOverheated;

			isOverheated =
				isOverheated
				&& !(heat <= 0)
				|| heat >= 1;
			
			if (isOverheated && !wasOverheated) {
				references.audioSource.loop = false;
				references.audioSource.Stop();
				references.audioSource.PlayOneShot(config.overheatSound);
			}
			
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
			fireInput = true;
			if (!isOverheated) {
				references.audioSource.PlayOneShot(config.startupSound);
				references.audioSource.clip = config.loopSound;
				references.audioSource.loop = true;
				references.audioSource.PlayScheduled(config.startupSound.length + AudioSettings.dspTime);
			}
		}
		
		public void ReleaseFire() {
			fireInput = false;
			if (isFiring) {
				references.audioSource.loop = false;
				references.audioSource.Stop();
				references.audioSource.PlayOneShot(config.shutdownSound);
			}
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
		}

		[Serializable]
		public struct References {
			public SpriteRenderer beam;
			public AudioSource audioSource;
		}
	}
}