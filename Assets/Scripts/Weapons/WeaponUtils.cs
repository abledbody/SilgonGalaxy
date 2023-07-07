using UnityEngine;

namespace SilgonGalaxy.Weapons {
	public static class WeaponUtils {
		public static T Attach<T>(string prefabName, Transform parent, Vector3 localPosition, float angle, System.Action<T> setup) where T : MonoBehaviour {
			if (Bootstrappers.WeaponCollection.TryFetch(prefabName, out var weapon)) {
				var instance = Object.Instantiate(weapon, localPosition, Quaternion.Euler(0, 0, angle), parent).GetComponent<T>();
				setup.Invoke(instance);
				return instance;
			}
			else
				throw new System.Exception($"Weapon {prefabName} not found");
		}
	}
}