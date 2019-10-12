// Copyright (c) 2015 burningmime
// 
// This software is provided 'as-is', without any express or implied
// warranty. In no event will the authors be held liable for any damages
// arising from the use of this software.
// 
// Permission is granted to anyone to use this software for any purpose,
// including commercial applications, and to alter it and redistribute it
// freely, subject to the following restrictions:
// 
// 1. The origin of this software must not be misrepresented; you must not
//    claim that you wrote the original software. If you use this software
//    in a product, an acknowledgement in the product documentation would be
//    appreciated but is not required.
// 2. Altered source versions must be plainly marked as such, and must not be
//    misrepresented as being the original software.
// 3. This notice may not be removed or altered from any source distribution.

using UnityEngine;
using System.Runtime.CompilerServices;

namespace burningmime.curves
{
    /// <summary>
    /// The point of this class is to abstract some of the functions of Vector2 so they can be used with System.Windows.Vector,
    /// System.Numerics.Vector2, UnityEngine.Vector2, or another vector type.
    /// </summary>
    public static class VectorHelper
    {
        /// <summary>
        /// Below this, don't trust the results of floating point calculations.
        /// </summary>
        public const float EPSILON = 1.2e-12f;

        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Distance(Vector3 a, Vector3 b) { return Vector3.Distance(a, b); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float DistanceSquared(Vector3 a, Vector3 b) { float tmp = Vector3.Distance(a, b); return tmp * tmp; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Dot(Vector3 a, Vector3 b) { return Vector3.Dot(a, b); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Normalize(Vector3 v) { return Vector3.Normalize(v); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float Length(Vector3 v) { return v.magnitude; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float LengthSquared(Vector3 v) { return v.sqrMagnitude; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static Vector3 Lerp(Vector3 a, Vector3 b, float amount) { return Vector3.Lerp(a, b, amount); }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float GetX(Vector3 v) { return v.x; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float GetY(Vector3 v) { return v.y; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] public static float GetZ(Vector3 v) { return v.z; }

        /// <summary>
        /// Checks if two vectors are equal within a small bounded error.
        /// </summary>
        /// <param name="v1">First vector to compare.</param>
        /// <param name="v2">Second vector to compare.</param>
        /// <returns>True iff the vectors are almost equal.</returns>
        #if !UNITY
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        #endif
        public static bool EqualsOrClose(Vector3 v1, Vector3 v2)
        {
            return DistanceSquared(v1, v2) < EPSILON;
        }
    }
}