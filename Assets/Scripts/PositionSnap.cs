using UnityEngine;

namespace SilgonGalaxy {
	using Extensions;

	public sealed class PositionSnap : MonoBehaviour {
		public Vector3 offset;

		private CameraDolly dolly;


		public void Awake() {
			dolly = Camera.main.GetComponent<CameraDolly>();
		}

		public void Update() {
			if (transform.parent == null) return;

			var camOffset = dolly.TruePosition - dolly.transform.position;

			transform.position = transform.parent.TransformPoint(offset).Quantize(Bootstrappers.GameConfig.pixelsPerUnit);
		}
	}
}