using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SilgonGalaxy {
	using Extensions;
	using Math = Extensions.MathExtensions;

	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class Ship : MonoBehaviour {
		private Rigidbody2D rb;

		public Config config;

		private float speed;
		private bool brakeLock;

		[NonSerialized]public float hInput;
		[NonSerialized]public float vInput;


		public void Awake() {
			rb = GetComponent<Rigidbody2D>();
		}

		public void FixedUpdate() {
			EvaluateSpeed(Time.fixedDeltaTime);
			EvaluateTurn(Time.fixedDeltaTime);
		}

		public void ThrustInput(InputAction.CallbackContext context) =>
			vInput = context.ReadValue<float>();
		public void TurnInput(InputAction.CallbackContext context) =>
			hInput = context.ReadValue<float>();

		public void EvaluateTurn(float dt) {
			// We're only considering the forward speed, as it's pretty much guaranteed to be higher than the reverse speed,
			// and we should still have some pretty reasonable responsiveness when going backwards.
			var responsiveness = 1 - speed.Abs() / config.topForwardSpeed * (1 - config.topSpeedSteering);

			var targetSpeed = hInput * config.topTurnSpeed * responsiveness;

			if (rb.angularVelocity * targetSpeed < 0 || targetSpeed == 0)
				rb.angularVelocity = 0;
			else
				rb.angularVelocity = rb.angularVelocity.MoveTowards(
					targetSpeed,
					config.turnAcceleration * responsiveness * dt
				);
		}

		public void EvaluateSpeed(float dt) {
			// If the signs are different, we're trying to slow down.
			var braking = speed * vInput < 0;
			var forward = vInput > 0;
			var backwards = vInput < 0;

			// Brake lock occurs when we have reached 0 speed,
			// but have not let up on the braking input.
			// This prevents the ship from overshooting 0 speed,
			// while still allowing the player to change direction
			// if they re-apply the input.
			brakeLock = (brakeLock && (backwards || forward)) || braking;
			
			float targetSpeed;
			float acceleration;
			(targetSpeed, acceleration) = true switch {
				// Braking is always trying to reach 0 speed.
				_ when braking => (0, config.braking),
				// Asymmetrical top-speeds and accelerations between forward and reverse.
				_ when forward && !brakeLock => (
					config.topForwardSpeed,
					// We add a small epsilon to avoid division by 0.
					config.topForwardSpeed / (config.forwardAccelerationTime + float.Epsilon)
				),
				_ when backwards && !brakeLock => (
					-config.topReverseSpeed,
					// We add a small epsilon to avoid division by 0.
					config.topReverseSpeed / (config.reverseAccelerationTime + float.Epsilon)
				),
				// No inputs means just coast.
				_ => (speed, 0)
			};

			speed = speed.MoveTowards(targetSpeed, acceleration * vInput.Abs() * dt);

			var velocityCorrection = dt/config.drift - dt;
			Debug.Log($"dt: {dt}, drift: {config.drift}, velocityCorrection: {velocityCorrection}");

			rb.velocity = rb.velocity.MoveTowards(
				transform.up * speed,
				velocityCorrection
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
			public float braking;
			[Min(0)]
			public float turnAcceleration;
			[Min(0)]
			public float topTurnSpeed;
			[Range(0, 1)]
			public float topSpeedSteering;
			[Range(0, 1)]
			public float drift;
		}
	}
}