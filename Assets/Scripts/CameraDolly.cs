using System;
using UnityEngine;

namespace SilgonGalaxy {
	using Extensions;

	public sealed class CameraDolly : MonoBehaviour {
		public Config config;
		
		[SerializeField] private Transform target;
		private Rigidbody2D targetRb;
		private Vector3 targetOffset;
		private Vector3 offset;
		private Vector3 truePosition;
		public Vector3 TruePosition => truePosition;


		public void Awake() {
			SetTarget(target);
			truePosition = transform.position;
			LookAhead();
			offset = targetOffset;
		}

		public void LateUpdate() {
			LookAhead();
			FollowTarget(Time.deltaTime);
			PositionSnap();
		}

		void PositionSnap() => transform.position = truePosition.Quantize(Bootstrappers.GameConfig.pixelsPerUnit);

		void FollowTarget(float dt) {
			if (target == null) return;
			
			var targetPosition = target.position + targetOffset;

			Debug.DrawLine(targetPosition + Vector3.down * 0.5f, targetPosition + Vector3.up * 0.5f, Color.gray);
			Debug.DrawLine(targetPosition + Vector3.left * 0.5f, targetPosition + Vector3.right * 0.5f, Color.gray);
			
			if (config.easing > 0) {
				offset = SmoothApproach(offset, targetOffset, config.easing, dt);
				truePosition = target.position + offset.Quantize(Bootstrappers.GameConfig.pixelsPerUnit);
			}
			else
				truePosition = targetPosition;
		}

		void LookAhead() {
			if (targetRb != null) {
				targetOffset = target.up * config.lookAhead + Vector3.back * config.distance;
				targetOffset += (Vector3)(targetRb.velocity * config.velocityTracking);
				targetOffset = targetOffset.Quantize(Bootstrappers.GameConfig.pixelsPerUnit);
			}
			else
				targetOffset = Vector3.zero;
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