using System;
using UnityEngine;

namespace SilgonGalaxy.Weapons {
	using Utils;
	using Extensions;

	[RequireComponent(typeof(SpriteRenderer), typeof(AudioSource))]
	public sealed class PlasmaCutter : MonoBehaviour, IStartStopWeapon {
		private const string PREFAB_NAME = "Plasma Cutter";
		
		private SpriteRenderer beam;
		private AudioSource audioSource;

		public Config config;
		
		public Clock flashClock;

		private float heat;
		private bool isFiring;
		private bool isOverheated;
		private bool CanFire => !isOverheated;


		public static PlasmaCutter Attach(Transform parent, Vector3 localPosition, float angle, Config config) {
			if (Bootstrappers.WeaponCollection.TryFetch(PREFAB_NAME, out var weapon)) {
				var instance = Instantiate(weapon, localPosition, Quaternion.Euler(0, 0, angle), parent).GetComponent<PlasmaCutter>();
				instance.config = config;
				return instance;
			}
			else
				throw new Exception($"Weapon {PREFAB_NAME} not found");
		}

		public void Awake() {
			beam = GetComponent<SpriteRenderer>();
			audioSource = GetComponent<AudioSource>();
		}

		public void Update() {
			isFiring = isFiring && CanFire;
			beam.enabled = isFiring;
			
			var (heatTarget, heatTime) = isFiring
				? (1, config.heatBuildupTime)
				: (0, config.heatShedTime);
			
			heat = Mathf.MoveTowards(heat, heatTarget, Time.deltaTime / heatTime);

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

			flashClock.Update(Time.deltaTime);
			beam.transform.localScale = new Vector3(
				beamWidth,
				1,
				1
			);
		}

		public void StartFire() {
			if (CanFire) {
				isFiring = true;
				audioSource.PlayOneShot(config.startupSound);
				audioSource.clip = config.loopSound;
				audioSource.loop = true;
				audioSource.PlayScheduled(config.startupSound.length + AudioSettings.dspTime);
			}
			else
				audioSource.PlayOneShot(config.startFailSound);
		}
		
		public void ReleaseFire() {
			if (!isFiring) return;
			audioSource.loop = false;
			audioSource.Stop();
			audioSource.PlayOneShot(config.shutdownSound);
			isFiring = false;
		}

		private void Overheat() {
			isOverheated = true;
			audioSource.loop = false;
			audioSource.Stop();
			audioSource.PlayOneShot(config.overheatSound);
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
	}
}