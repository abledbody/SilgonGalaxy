using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SilgonGalaxy {
	using Extensions;
	using Weapons;

	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class Ship : MonoBehaviour {
		private Rigidbody2D rb;

		public Config config;

		public ChargeBlaster chargeBlaster = new();

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
			chargeBlaster.Update(Time.fixedDeltaTime, rb);
		}

		public void ThrustInput(InputAction.CallbackContext context) =>
			vInput = context.ReadValue<float>();
		public void TurnInput(InputAction.CallbackContext context) =>
			hInput = context.ReadValue<float>();
		
		public void FireInput(InputAction.CallbackContext context) {
			if (context.started)
				chargeBlaster.StartFire(rb);
			if (context.canceled)
				chargeBlaster.ReleaseFire(rb);
		}

		public void EvaluateTurn(float dt) {
			// We're only considering the forward speed, as it's pretty much guaranteed to be higher than the reverse speed,
			// and we should still have some pretty reasonable responsiveness when going backwards.
			var responsiveness = 1 - speed.Abs() / config.topForwardSpeed * (1 - config.topSpeedSteering);

			var targetSpeed = hInput * config.topTurnSpeed * responsiveness;

			// We instantly snap to 0 angular velocity if we're not actively trying to turn into the current direction of rotation.
			if (targetSpeed == 0 || targetSpeed.DifferentSign(rb.angularVelocity))
				rb.angularVelocity = 0;
			else
				// It takes time to get to a higher angular velocity.
				rb.angularVelocity = rb.angularVelocity.MoveTowards(
					targetSpeed,
					config.turnAcceleration * responsiveness * dt
				);
		}

		public void EvaluateSpeed(float dt) {
			var braking = vInput.DifferentSign(speed);
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
				transform.up * speed,
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