using UnityEngine;

namespace SilgonGalaxy.Weapons {
	using Utils;

	[System.Serializable]
	public sealed class ChargeBlaster {
		public Config config;
		public References references;

		private Delay chargeDelay;
		private bool isFiring;
		private bool isShotBuffered;
		private Delay shotDelay;


		public void Update(float dt, Rigidbody2D rb) {
			chargeDelay.Update(dt);
			
			if (shotDelay.Update(dt) && isFiring) {
				if (isShotBuffered) {
					references.audioSource.PlayOneShot(config.smallShotSound);
					SpawnProjectile(rb, config.smallProjectile);
					chargeDelay.Start(config.chargeTime);
					isShotBuffered = false;
				}
				else {
					references.audioSource.PlayOneShot(config.initialChargeSound);
					references.audioSource.clip = config.chargeLoopSound;
					references.audioSource.loop = true;
					references.audioSource.PlayScheduled(config.initialChargeSound.length + AudioSettings.dspTime);
				}
			}
		}

		public void StartFire(Rigidbody2D rb) {
			isFiring = true;

			if (!shotDelay.Complete) {
				isShotBuffered = true;
				return;
			}
			chargeDelay.Start(config.chargeTime);
			
			references.audioSource.PlayOneShot(config.smallShotSound);
			SpawnProjectile(rb, config.smallProjectile);
		}
		
		public void ReleaseFire(Rigidbody2D rb) {
			if (!isFiring) return;
			isFiring = false;
			
			if (!shotDelay.Complete) return;
			
			(Projectile prefab, AudioClip clip) = 
				chargeDelay.Complete
				? (config.bigProjectile, config.bigShotSound)
				: (config.smallProjectile, config.smallShotSound);
			references.audioSource.Stop();
			references.audioSource.PlayOneShot(clip);
			SpawnProjectile(rb, prefab);
		}

		private void SpawnProjectile(Rigidbody2D rb, Projectile prefab) {
			shotDelay.Start(config.shotInterval);
			Projectile projectile = Object.Instantiate(prefab, references.spawnPosition.position, references.spawnPosition.rotation);
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
			public Transform spawnPosition;
			public AudioSource audioSource;
		}
	}
}