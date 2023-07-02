using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace SilgonGalaxy.Ships {
	using Weapons;

	[RequireComponent(typeof(Rigidbody2D))]
	public class Junker : MonoBehaviour {
		private Rigidbody2D rb;

		[SerializeField] internal Thrusters thrusters = new();
		[SerializeField] internal Rotation rotation = new();

		[NonSerialized]public float hInput;
		[NonSerialized]public float vInput;


		public void Awake() {
			rb = GetComponent<Rigidbody2D>();
		}

		public void FixedUpdate() {
			thrusters.EvaluateSpeed(rb, vInput, Time.fixedDeltaTime);
			rotation.EvaluateTurn(rb, thrusters.TopSpeedFraction, hInput, Time.fixedDeltaTime);
		}

		public void ThrustInput(InputAction.CallbackContext context) =>
			vInput = context.ReadValue<float>();
		public void TurnInput(InputAction.CallbackContext context) =>
			hInput = context.ReadValue<float>();
		public void FireInput(InputAction.CallbackContext context) {throw new NotImplementedException();}
	}
}