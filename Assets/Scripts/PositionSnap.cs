using UnityEngine;

namespace SilgonGalaxy {
	using Extensions;

	public sealed class PositionSnap : MonoBehaviour {
		public GameConfig gameConfig;
		public Vector3 offset;

		private CameraDolly dolly;


		public void Awake() {
			GameConfig.CheckAssigned(ref gameConfig, this);
			dolly = Camera.main.GetComponent<CameraDolly>();
		}

		public void Update() {
			if (transform.parent == null) return;

			var camOffset = dolly.TruePosition - dolly.transform.position;

			transform.position = transform.parent.TransformPoint(offset).Quantize(gameConfig.pixelsPerUnit);
		}
	}
}