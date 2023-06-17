using UnityEngine;
using Math = Unity.Mathematics.math;

namespace SilgonGalaxy.Extensions {
	public static class MathExtensions {
		public const float TAU = Mathf.PI * 2f;

		public static float Abs(this float value) => Math.abs(value);
		public static float Sign(this float value) => Math.sign(value);
		
		public static float Sqr(this int value) => value * value;
		public static float Sqr(this float value) => value * value;
		public static float Sqrt(this float value) => Math.sqrt(value);

		public static float Floor(this float val) => Math.floor(val);
		public static float Ceil(this float val) => Math.ceil(val);
		public static float Round(this float val) => Math.round(val);
		public static int FloorToInt(this float val) => Mathf.FloorToInt(val);
		public static int CeilToInt(this float val) => Mathf.CeilToInt(val);
		public static int RoundToInt(this float val) => Mathf.RoundToInt(val);

		public static float Min(this float a, float b) => Math.min(a, b);
		public static float Max(this float a, float b) => Math.max(a, b);
		public static int Saturate(this int val) => (int)Math.saturate(val);
		public static float Saturate(this float val) => Math.saturate(val);
		public static float Clamp(this float val, float min, float max) => Math.clamp(val, min, max);
		public static int Clamp(this int val, int min, int max) => Math.clamp(val, min, max);
		public static int ClampToInt(this float val, int min, int max) => val.Clamp(min, max).RoundToInt();

		public static int Mod(this int a, int b) => (a % b + b) % b;
		public static float Mod(this float a, float b) => (a % b + b) % b;

		public static float Comp(this float lhs, float rhs) => lhs * rhs.Sign();

		public static int ToInt(this bool val) => val ? 1 : 0;
		public static bool ToBool(this int val) => val != 0;

		public static byte ToByte(this bool val) => (byte)(val ? 1 : 0);
		public static bool ToBool(this byte val) => val != 0;

		public static float MoveTowards(this float current, float target, float maxDelta) => Mathf.MoveTowards(current, target, maxDelta);

		/// <summary>Returns the value with the lowest magnitude.</summary>
		/// <param name="values">Each value to be compared</param>
		/// <returns>The value with the lowest magnitude.</returns>
		public static float MagnitudeMin(params float[] values) {
			var min = float.PositiveInfinity;
			foreach (var value in values) { min = Mathf.Abs(value) < Mathf.Abs(min) ? value : min; }
			return min;
		}

		/// <summary>Converts a FOV value in degrees into a slope.</summary>
		/// <param name="fov">The FOV value in degrees.</param>
		/// <returns>The slope of half the FOV.</returns>
		public static float FOVToSlope(float fov) => Mathf.Tan(fov * Mathf.Deg2Rad / 2);

		/// <summary>Converts a slope into a FOV value in degrees.</summary>
		/// <param name="slope">The slope of half the FOV.</param>
		/// <returns>The FOV value in degrees.</returns>
		public static float SlopeToFOV(float slope) => Mathf.Atan(slope) * Mathf.Rad2Deg * 2;

		public static Vector2 Perpendicular(this Vector2 vec) => Vector2.Perpendicular(vec);
		public static float Dot(this Vector2 lhs, Vector2 rhs) => Vector2.Dot(lhs, rhs);
		public static float Dot(this Vector3 lhs, Vector3 rhs) => Vector3.Dot(lhs, rhs);
		public static float Comp(this Vector2 vector, Vector2 normal) => Vector2.Dot(vector, normal.normalized);
		public static Vector2 Project(this Vector2 vector, Vector2 normal) => normal * Vector2.Dot(vector, normal);
		public static Vector2 ProjectAutonormal(this Vector2 vector, Vector2 onto) => onto * Vector2.Dot(vector, onto) / onto.sqrMagnitude;
		public static float Atan2(this Vector2 vector) => Mathf.Atan2(vector.y, vector.x);
		public static float Atan2(this Vector3 vector) => Mathf.Atan2(vector.y, vector.x);
	}
}