using UnityEngine;

namespace SilgonGalaxy {
	[ExecuteInEditMode]
	public class PixelScaler : MonoBehaviour {
		public Texture texture;
		public Color clearColor;

		Rect screenRect;
		Vector2Int screenSize;
		int multiplier = 1;
		

		void Awake() {
			ScreenRectUpdate();
		}
		
		void OnGUI() {
			GL.Clear(true, true, clearColor);
			
			// To avoid changing GL.sRGBWrite for other graphics we'll store what it was set to previously.
			bool buffer = GL.sRGBWrite;

			// RenderTextures default to a linear colorspace, which when drawn with DrawTexture looks too dark.
			// To fix this we do the conversion between linear and sRGB for this one DrawTexture operation.
			GL.sRGBWrite = true;
			Graphics.DrawTexture(screenRect, texture);

			GL.sRGBWrite = buffer;
		}

		private void CalculateScreenSize() {
			screenSize = new Vector2Int(Screen.width, Screen.height);

			// We want the pixel multiplier to be as large as possible without making screenRect larger than screenSize
			// in either dimension, unless it would mean shrinking the screen to less than one screen pixel per rendered pixel.
			multiplier = Mathf.Max(
				Mathf.Min(
					Mathf.FloorToInt(screenSize.x / texture.width),
					Mathf.FloorToInt(screenSize.y / texture.height)
					), 1);
			
			if (Screen.fullScreen && !Application.isEditor) {
				screenSize = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);
			}
		}

		private void LateUpdate() {
			if (screenSize != new Vector2Int(Screen.width, Screen.height)) {
				CalculateScreenSize();
				ScreenRectUpdate();
			}
		}

		void ScreenRectUpdate() {
			Vector2Int rectScale = new Vector2Int(texture.width * multiplier, texture.height * multiplier);

			screenRect = new Rect((screenSize - rectScale)/2, rectScale);
		}
	}
}