using UnityEngine;

namespace SilgonGalaxy {
	[CreateAssetMenu(fileName = "GameConfig", menuName = "SilgonGalaxy/GameConfig", order = 0)]
	public class GameConfig : ScriptableObject {
		public float pixelsPerUnit = 12;

		public static void CheckAssigned(ref GameConfig config, MonoBehaviour context) {
			if (config == null) {
				Debug.LogError($"{context.name} has no {nameof(GameConfig)} assigned.");
				context.enabled = false;
			}
		}
	}
}