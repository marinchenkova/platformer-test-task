using System.Runtime.CompilerServices;
using UnityEngine;

namespace Platformer.Utils {

    public static class MathUtils {

        public static readonly float TOLERANCE = Mathf.Epsilon;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this float value) {
            return Mathf.Abs(value) <= TOLERANCE;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this float value, float tolerance) {
            return Mathf.Abs(value) <= tolerance;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyEqual(this float value, float other) {
            return IsNearlyZero(value - other);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyEqual(this float value, float other, float tolerance) {
            return IsNearlyZero(value - other, tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector3 vector) {
            return vector.x.IsNearlyZero() && vector.y.IsNearlyZero() && vector.z.IsNearlyZero();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector3 vector, float tolerance) {
            return vector.x.IsNearlyZero(tolerance) && vector.y.IsNearlyZero(tolerance) && vector.z.IsNearlyZero(tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector2 vector) {
            return vector.x.IsNearlyZero() && vector.y.IsNearlyZero();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyZero(this Vector2 vector, float tolerance) {
            return vector.x.IsNearlyZero(tolerance) && vector.y.IsNearlyZero(tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyEqual(this Vector3 vector, Vector3 other) {
            return vector.x.IsNearlyEqual(other.x) && vector.y.IsNearlyEqual(other.y) && vector.z.IsNearlyEqual(other.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyEqual(this Vector3 vector, Vector3 other, float tolerance) {
            return vector.x.IsNearlyEqual(other.x, tolerance) && vector.y.IsNearlyEqual(other.y, tolerance) && vector.z.IsNearlyEqual(other.z, tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyEqual(this Vector2 vector, Vector2 other) {
            return vector.x.IsNearlyEqual(other.x) && vector.y.IsNearlyEqual(other.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsNearlyEqual(this Vector2 vector, Vector2 other, float tolerance) {
            return vector.x.IsNearlyEqual(other.x, tolerance) && vector.y.IsNearlyEqual(other.y, tolerance);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 Inverted(this Vector3 vector) {
            return -1f * vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 Inverted(this Vector2 vector) {
            return -1f * vector;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithX(this Vector3 vector, float x) {
            return new Vector3(x, vector.y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithY(this Vector3 vector, float y) {
            return new Vector3(vector.x, y, vector.z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector3 WithZ(this Vector3 vector, float z) {
            return new Vector3(vector.x, vector.y, z);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithX(this Vector2 vector, float x) {
            return new Vector2(x, vector.y);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector2 WithY(this Vector2 vector, float y) {
            return new Vector2(vector.x, y);
        }
    }

}
