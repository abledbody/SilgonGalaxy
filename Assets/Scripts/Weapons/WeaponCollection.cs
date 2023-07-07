using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SilgonGalaxy {
	[System.Serializable]
	public sealed class WeaponCollection {
		const string WEAPON_LABEL = "Weapon";

		private Dictionary<string, GameObject> weapons;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Populate() {
			Dictionary<string, GameObject> weapons = new();
			Bootstrappers.WeaponCollection.weapons = weapons;

			foreach (var weapon in Addressables.LoadAssetsAsync<GameObject>(WEAPON_LABEL, null).WaitForCompletion())
				weapons.Add(weapon.name, weapon);
		}

		public bool TryFetch(string name, out GameObject weapon) => weapons.TryGetValue(name, out weapon);
	}
}
