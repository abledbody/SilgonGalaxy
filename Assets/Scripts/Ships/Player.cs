using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SilgonGalaxy.Ships {
	using Weapons;

	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class Player : MonoBehaviour {
		private Rigidbody2D rb;

		[SerializeField] internal Thrusters thrusters = new();
		[SerializeField] internal Rotation rotation = new();

		public ChargeBlaster chargeBlaster = new();
		
		[NonSerialized]public float hInput;
		[NonSerialized]public float vInput;


		public void Awake() {
			rb = GetComponent<Rigidbody2D>();
		}

		public void FixedUpdate() {
			thrusters.EvaluateSpeed(rb, vInput, Time.fixedDeltaTime);
			rotation.EvaluateTurn(rb, thrusters.TopSpeedFraction, hInput, Time.fixedDeltaTime);
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
	}
}