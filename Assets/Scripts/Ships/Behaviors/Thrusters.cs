using System;
using UnityEngine;

namespace SilgonGalaxy.Ships {
	using Extensions;
	
	[Serializable]
	internal sealed class Thrusters {
		public Config config;

		private float speed;
		private bool brakeLock;

		public float TopSpeedFraction => speed.Abs() / config.topForwardSpeed;


		public void EvaluateSpeed(Rigidbody2D rb, float vInput, float dt) {
			var braking = vInput.DifferentSign(speed);
			var forward = vInput > 0;
			var backwards = vInput < 0;

			// Brake lock occurs when we have reached 0 speed,
			// but have not let up on the braking input.
			// This prevents the ship from overshooting 0 speed,
			// while still allowing the player to change direction
			// if they re-apply the input.
			brakeLock = config.useBrakeLock && (brakeLock && (backwards || forward) || braking);
			
			float targetSpeed;
			float acceleration;
			(targetSpeed, acceleration) = true switch {
				// Braking is always trying to reach 0 speed.
				_ when braking => (0, config.topForwardSpeed / (config.brakingTime + float.Epsilon)),
				// Top speed and acceleration are asymmetrical between forward and reverse.
				_ when forward && !brakeLock => (
					config.topForwardSpeed,
					// We add an epsilon to avoid division by 0.
					config.topForwardSpeed / (config.forwardAccelerationTime + float.Epsilon)
				),
				_ when backwards && !brakeLock => (
					-config.topReverseSpeed,
					// We add an epsilon to avoid division by 0.
					config.topReverseSpeed / (config.reverseAccelerationTime + float.Epsilon)
				),
				// No inputs means just coast.
				_ => (speed, 0)
			};

			speed = speed.MoveTowards(targetSpeed, acceleration * vInput.Abs() * dt);

			// Doing this allows the ship to drift a bit when turning.
			rb.velocity = rb.velocity.MoveTowards(
				rb.transform.up * speed,
				dt / config.drift - dt // Equivalent to:  (1 / drift - 1) * dt
			);
		}

		[Serializable]
		public struct Config {
			[Min(0)]
			public float topForwardSpeed;
			[Min(0)]
			public float topReverseSpeed;
			[Min(0)]
			public float forwardAccelerationTime;
			[Min(0)]
			public float reverseAccelerationTime;
			[Min(0)]
			public float brakingTime;
			[Min(0)]
			public float drift;
			public bool useBrakeLock;
		}
	}
}