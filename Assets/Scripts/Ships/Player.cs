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
		
		[NonSerialized]public float hInput;
		[NonSerialized]public float vInput;

		public PlasmaCutter.Config plasmaCutterConfig;
		private PlasmaCutter plasmaCutter;


		public void Awake() {
			rb = GetComponent<Rigidbody2D>();
			plasmaCutter = PlasmaCutter.Attach(transform, Vector3.zero, 0, plasmaCutterConfig);
		}

		public void FixedUpdate() {
			thrusters.EvaluateSpeed(rb, vInput, Time.fixedDeltaTime);
			rotation.EvaluateTurn(rb, thrusters.TopSpeedFraction, hInput, Time.fixedDeltaTime);
		}

		public void ThrustInput(InputAction.CallbackContext context) =>
			vInput = context.ReadValue<float>();
		public void TurnInput(InputAction.CallbackContext context) =>
			hInput = context.ReadValue<float>();
		
		public void FireInput(InputAction.CallbackContext context) {
			if (context.started)
				plasmaCutter.StartFire();
			if (context.canceled)
				plasmaCutter.ReleaseFire();
		}
	}
}