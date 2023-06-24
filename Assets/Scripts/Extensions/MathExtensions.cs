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
		public static float Quantize(this float val, float interval) => (val * interval).Round() / interval;

		public static int Mod(this int a, int b) => (a % b + b) % b;
		public static float Mod(this float a, float b) => (a % b + b) % b;

		public static float Comp(this float lhs, float rhs) => lhs * rhs.Sign();
		public static bool DifferentSign(this float lhs, float rhs) => lhs * rhs < 0;

		public static int ToInt(this bool val) => val ? 1 : 0;
		public static bool ToBool(this int val) => val != 0;

		public static byte ToByte(this bool val) => (byte)(val ? 1 : 0);
		public static bool ToBool(this byte val) => val != 0;

		public static float MoveTowards(this float current, float target, float maxDelta) => Mathf.MoveTowards(current, target, maxDelta);
		public static float Lerp(this float a, float b, float t) => Mathf.Lerp(a, b, t);

		public static float Deg(this float rad) => Math.degrees(rad);
		public static float Rad(this float deg) => Math.radians(deg);

		/// <summary>Returns the value with the lowest magnitude.</summary>
		/// <param name="values">Each value to be compared</param>
		/// <returns>The value with the lowest magnitude.</returns>
		public static float MagnitudeMin(params float[] values) {
			var min = float.PositiveInfinity;
			foreach (var value in values) { min = value.Abs() < min.Abs() ? value : min; }
			return min;
		}

		/// <summary>Converts a FOV value in degrees into a slope.</summary>
		/// <param name="fov">The FOV value in degrees.</param>
		/// <returns>The slope of half the FOV.</returns>
		public static float FOVToSlope(float fov) => Math.tan(Math.radians(fov) / 2);

		/// <summary>Converts a slope into a FOV value in degrees.</summary>
		/// <param name="slope">The slope of half the FOV.</param>
		/// <returns>The FOV value in degrees.</returns>
		public static float SlopeToFOV(float slope) => Math.degrees(Math.atan(slope)) * 2;

		public static Vector2 Perpendicular(this Vector2 vec) => Vector2.Perpendicular(vec);
		public static float Dot(this Vector2 lhs, Vector2 rhs) => Vector2.Dot(lhs, rhs);
		public static float Dot(this Vector3 lhs, Vector3 rhs) => Vector3.Dot(lhs, rhs);
		public static Vector2 Lerp(this Vector2 a, Vector2 b, float t) => Vector2.Lerp(a, b, t);
		public static Vector3 Lerp(this Vector3 a, Vector3 b, float t) => Vector3.Lerp(a, b, t);
		public static Vector2 MoveTowards(this Vector2 current, Vector2 target, float maxDelta) => Vector2.MoveTowards(current, target, maxDelta);
		public static Vector3 MoveTowards(this Vector3 current, Vector3 target, float maxDelta) => Vector3.MoveTowards(current, target, maxDelta);
		public static float Comp(this Vector2 vector, Vector2 normal) => Vector2.Dot(vector, normal.normalized);
		public static Vector2 Project(this Vector2 vector, Vector2 normal) => normal * Vector2.Dot(vector, normal);
		public static Vector2 ProjectAutonormal(this Vector2 vector, Vector2 onto) => onto * Vector2.Dot(vector, onto) / onto.sqrMagnitude;
		public static float Atan2(this Vector2 vector) => Math.atan2(vector.y, vector.x);
		public static float Atan2(this Vector3 vector) => Math.atan2(vector.y, vector.x);
		public static Vector2 Direction2(this float angle) => new(Math.cos(angle), Math.sin(angle));
		public static Vector3 Round(this Vector3 vector) => new(vector.x.Round(), vector.y.Round(), vector.z.Round());
		public static Vector3 Quantize(this Vector3 vector, float interval) => (vector * interval).Round() / interval;
	}
}