using UnityEngine;
using UnityEngine.AddressableAssets;

namespace SilgonGalaxy {
	[CreateAssetMenu(fileName = "Bootstrappers", menuName = "SilgonGalaxy/Bootstrappers")]
	public sealed class Bootstrappers : ScriptableObject {
		public const string ADDRESS = "Bootstrappers";

		private static Bootstrappers instance;
		
		[SerializeField] private GameConfig gameConfig;
		public static GameConfig GameConfig => instance.gameConfig;
		[SerializeField] private WeaponCollection weaponCollection;
		public static WeaponCollection WeaponCollection => instance.weaponCollection;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		private static void Initialize() {
			instance = Addressables.LoadAssetAsync<Bootstrappers>(ADDRESS).WaitForCompletion();
		}
	}
}