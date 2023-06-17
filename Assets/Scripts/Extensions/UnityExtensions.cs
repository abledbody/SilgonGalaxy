using System;
using System.Linq;
using System.Collections.Generic;

using UnityEngine;

namespace SilgonGalaxy.Extensions {
	public static class UnityExtensions {
		private static readonly ContactFilter2D blankContactFilter = new();
		
		/// <summary>Returns all points of contact with the rigidbody using the specified filter.</summary>
		public static IEnumerable<ContactPoint2D> GetAllContacts(this Rigidbody2D rb, ContactFilter2D filter) {
			var colliders = new List<Collider2D>();
			rb.GetAttachedColliders(colliders);
			
			IEnumerable<ContactPoint2D> ContactExtractor(Collider2D collider) {
				var contacts = new List<ContactPoint2D>();
				collider.GetContacts(filter, contacts);
				return contacts;
			};
			
			return colliders.SelectMany(ContactExtractor);
		}

		/// <summary>Returns all points of contact with the rigidbody.</summary>
		public static IEnumerable<ContactPoint2D> GetAllContacts(this Rigidbody2D rb) {
			return rb.GetAllContacts(blankContactFilter);
		}
		
		/// <summary>Checks if the layer mask contains the specified layer index.</summary>
		/// <param name="mask">The layer mask to check.</param>
		/// <param name="layer">The index of the layer to check for.</param>
		public static bool Contains(this LayerMask mask, int layer) => mask == (mask | (1 << layer));

		public static bool CheckOverlap(this Collider2D[] targetColliders, ContactFilter2D contactFilter) {
			// Don't care what goes in here.
			var blankArray = new Collider2D[1];

			for (int i = 0; i < targetColliders.Length; i++) {
				// OverlapCollider tells us how many colliders it filled the array with.
				// Can't ever return false here, or else the loop is pointless.
				if (targetColliders[i].OverlapCollider(contactFilter, blankArray) > 0) 
					return true;
			}

			return false;
		}

		public static Rect GetCanvasSpaceRect(this RectTransform val) {
			Vector2 size = Vector2.Scale(val.rect.size, val.lossyScale);
			return new(
				val.position.x - (val.pivot.x * size.x),
				val.position.y - (val.pivot.y * size.y),
				size.x,
				size.y
			);
		}
	}
}