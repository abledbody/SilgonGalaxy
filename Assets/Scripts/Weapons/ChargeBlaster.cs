using UnityEngine;

namespace SilgonGalaxy.Weapons {
	using Utils;

	[RequireComponent(typeof(AudioSource))]
	public sealed class ChargeBlaster : MonoBehaviour, IStartStopWeapon {
		private const string PREFAB_NAME = "Charge Blaster";

		private AudioSource audioSource;
		
		public Config config;
		public References refs;

		private Delay chargeDelay;
		private bool isFiring;
		private bool isShotBuffered;
		private Delay shotDelay;


		public static ChargeBlaster Attach(Transform parent, Vector3 localPosition, float angle, Config config, References refs) =>
			WeaponUtils.Attach<ChargeBlaster>(PREFAB_NAME, parent, localPosition, angle, instance => {
				instance.config = config;
				instance.refs = refs;
			});

		public void Awake() {
			audioSource = GetComponent<AudioSource>();
		}

		public void Update() {
			chargeDelay.Update(Time.deltaTime);
			
			if (shotDelay.Update(Time.deltaTime) && isFiring) {
				if (isShotBuffered) {
					audioSource.PlayOneShot(config.smallShotSound);
					SpawnProjectile(refs.rb, config.smallProjectile);
					chargeDelay.Start(config.chargeTime);
					isShotBuffered = false;
				}
				else {
					audioSource.PlayOneShot(config.initialChargeSound);
					audioSource.clip = config.chargeLoopSound;
					audioSource.loop = true;
					audioSource.PlayScheduled(config.initialChargeSound.length + AudioSettings.dspTime);
				}
			}
		}

		public void StartFire() {
			isFiring = true;

			if (!shotDelay.Complete) {
				isShotBuffered = true;
				return;
			}
			chargeDelay.Start(config.chargeTime);
			
			audioSource.PlayOneShot(config.smallShotSound);
			SpawnProjectile(refs.rb, config.smallProjectile);
		}
		
		public void ReleaseFire() {
			if (!isFiring) return;
			isFiring = false;
			
			if (!shotDelay.Complete) return;
			
			(Projectile prefab, AudioClip clip) = 
				chargeDelay.Complete
				? (config.bigProjectile, config.bigShotSound)
				: (config.smallProjectile, config.smallShotSound);
			audioSource.Stop();
			audioSource.PlayOneShot(clip);
			SpawnProjectile(refs.rb, prefab);
		}

		private void SpawnProjectile(Rigidbody2D rb, Projectile prefab) {
			shotDelay.Start(config.shotInterval);
			Projectile projectile = Instantiate(prefab, transform.position, transform.rotation);
			projectile.Init(rb);
		}

		[System.Serializable]
		public struct Config {
			public float shotInterval;
			[Min(float.Epsilon)]
			public float chargeTime;

			public Projectile smallProjectile;
			public Projectile bigProjectile;

			public AudioClip smallShotSound;
			public AudioClip bigShotSound;
			public AudioClip initialChargeSound;
			public AudioClip chargeLoopSound;
		}

		[System.Serializable]
		public struct References {
			public Rigidbody2D rb;
		}
	}
}