using UnityEngine;

namespace SilgonGalaxy.Weapons {
	using Utils;

	[System.Serializable]
	public sealed class ChargeBlaster {
		public Config config;

		private Delay chargeDelay;
		private bool firing;
		private bool bufferedShot;
		private Delay shotDelay;


		public void Update(float dt, Rigidbody2D rb) {
			chargeDelay.Update(dt);
			
			if (shotDelay.Update(dt)) {
				if (firing && bufferedShot) {
					SpawnProjectile(rb, config.smallProjectile);
					chargeDelay.Start(config.chargeTime);
					bufferedShot = false;
				}
			}
		}

		public void StartFire(Rigidbody2D rb) {
			firing = true;

			if (!shotDelay.Complete) {
				bufferedShot = true;
				return;
			}
			chargeDelay.Start(config.chargeTime);

			SpawnProjectile(rb, config.smallProjectile);
		}
		
		public void ReleaseFire(Rigidbody2D rb) {
			if (!firing || !shotDelay.Complete) return;

			firing = false;
			
			Projectile projectilePrefab = 
				chargeDelay.Complete
				? config.bigProjectile
				: config.smallProjectile;
			
			SpawnProjectile(rb, projectilePrefab);
		}

		private void SpawnProjectile(Rigidbody2D rb, Projectile prefab) {
			shotDelay.Start(config.shotInterval);
			Projectile projectile = Object.Instantiate(prefab, rb.transform.TransformPoint(config.offset), rb.transform.rotation);
			projectile.Init(rb);
		}

		[System.Serializable]
		public struct Config {
			public float shotInterval;
			[Min(float.Epsilon)]
			public float chargeTime;
			public Vector2 offset;

			public Projectile smallProjectile;
			public Projectile bigProjectile;
		}
	}
}