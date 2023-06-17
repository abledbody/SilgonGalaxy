using System;
using UnityEngine;

namespace SilgonGalaxy {
	public sealed class CameraDolly : MonoBehaviour {
		public Config config;
		[SerializeField] private Transform target;
		private Rigidbody2D targetRb;
		private Vector3 offset;
		private Vector3 truePosition;


		public void Awake() {
			SetTarget(target);
		}

		public void Update() {
			LookAhead();
			FollowTarget(Time.deltaTime);
		}

		void FollowTarget(float dt) {
			if (target == null) return;

			var targetWithOffset = target.position + offset;

			if (config.easing > 0)
				transform.position = SmoothApproach(transform.position, targetWithOffset, config.easing, dt);
			else
				transform.position = targetWithOffset;
		}

		void LookAhead() {
			offset = target.up * config.lookAhead + Vector3.back * config.distance;

			if (targetRb == null) return;

			offset += (Vector3)(targetRb.velocity * config.velocityTracking);
		}
		
		public void SetTarget(Transform target) {
			this.target = target;
			targetRb = target.GetComponent<Rigidbody2D>();
		}

		private static float SmoothApproach(float value, float target, float smoothness, float dt)
			=> (value - target) / (dt / smoothness + 1) + target;

		private static Vector3 SmoothApproach(Vector3 value, Vector3 target, float smoothness, float dt)
			=> (value - target) / (dt / smoothness + 1) + target;

		[Serializable]
		public struct Config {
			public float easing;
			public float lookAhead;
			public float velocityTracking;
			public float distance;
		}
	}
}