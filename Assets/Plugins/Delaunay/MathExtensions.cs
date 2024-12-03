/**
 * Copyright 2019 Oskar Sigvardsson
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

namespace Unity.Mathematics
{
    public static class MathExtensions
    {
        public static bool IsReal(this float f)
        {
            return !float.IsInfinity(f) && !float.IsNaN(f);
        }

        public static bool IsReal(this float2 v2)
        {
            return v2.x.IsReal() && v2.y.IsReal();
        }

        public static bool IsReal(this float3 v3)
        {
            return v3.x.IsReal() && v3.y.IsReal() && v3.z.IsReal();
        }

        public static float2 ToFloat2(this float3 v2)
        {
            return new float2(v2.x, v2.y);
        }

        public static float3 ToFloat3(this float2 v2)
        {
            return new float3(v2.x, v2.y, 0);
        }
    }
}