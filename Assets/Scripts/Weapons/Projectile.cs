using UnityEngine;
using System.Collections.Generic;

namespace SilgonGalaxy.Weapons {
	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class Projectile : MonoBehaviour {
		private Rigidbody2D rb;
		private Collider2D[] colliders;
		[System.NonSerialized] public Rigidbody2D origin;

		public Config config;

		private ContactFilter2D contactFilter;

		public void Awake() {
			rb = GetComponent<Rigidbody2D>();
			colliders = new Collider2D[rb.attachedColliderCount];
			rb.GetAttachedColliders(colliders);

			contactFilter = new ContactFilter2D {
				useLayerMask = true,
				layerMask = config.hitMask
			};

			// Ignore collisions with the origin.
			if (origin != null) {
				List<Collider2D> originColliders = new();
				origin.GetAttachedColliders(originColliders);

				foreach (var collider in originColliders) {
					foreach (var otherCollider in colliders)
						Physics2D.IgnoreCollision(collider, otherCollider);
				}
			}
		}
		
		public void Start() {
			Destroy(gameObject, config.lifetime);
		}
		
		public void FixedUpdate() {
			List<RaycastHit2D> hits = new();

			// We're only concerned with the first hit, so we can break out of the loop as soon as we find one.
			foreach (var collider in colliders)
				if (collider.Cast(transform.up, contactFilter, hits, config.speed * Time.fixedDeltaTime) > 0) break;

			if (hits.Count > 0)
				Hit(hits[0].collider.gameObject);
		}

		public void Init(Rigidbody2D rb) {
			origin = rb;
			this.rb.velocity = rb.velocity + (Vector2) rb.transform.up * config.speed;
		}

		private void Hit(GameObject obj) {
			Debug.Log($"Hit {obj.name}");
			Destroy(gameObject);
		}


		[System.Serializable]
		public struct Config {
			public float speed;
			public float lifetime;
			public LayerMask hitMask;
		}
	}
}