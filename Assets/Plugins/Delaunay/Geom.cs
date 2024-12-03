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

using System.Collections.Generic;
using Unity.Mathematics;
using Random = Unity.Mathematics.Random;

namespace Delaunay
{
    public class Geom
    {
        /// <summary>
        /// Are these two vectors (approximately) coincident
        /// </summary>
        public static bool AreCoincident(float2 a, float2 b)
        {
            return math.distance(a, b) < 0.000001f;
        }

        /// <summary>
        /// Is point p to the left of the line from l0 to l1?
        /// </summary>
        public static bool ToTheLeft(float2 p, float2 l0, float2 l1)
        {
            return ((l1.x - l0.x) * (p.y - l0.y) - (l1.y - l0.y) * (p.x - l0.x)) >= 0;
        }

        /// <summary>
        /// Is point p to the right of the line from l0 to l1?
        /// </summary>
        public static bool ToTheRight(float2 p, float2 l0, float2 l1)
        {
            return !ToTheLeft(p, l0, l1);
        }

        /// <summary>
        /// Is point p inside the triangle formed by c0, c1 and c2 (assuming c1,
        /// c2 and c3 are in CCW order)
        /// </summary>
        public static bool PointInTriangle(float2 p, float2 c0, float2 c1, float2 c2)
        {
            return ToTheLeft(p, c0, c1)
                   && ToTheLeft(p, c1, c2)
                   && ToTheLeft(p, c2, c0);
        }

        /// <summary>
        /// Is point p inside the circumcircle formed by c0, c1 and c2?
        /// </summary>
        public static bool InsideCircumcircle(float2 p, float2 c0, float2 c1, float2 c2)
        {
            var ax = c0.x - p.x;
            var ay = c0.y - p.y;
            var bx = c1.x - p.x;
            var by = c1.y - p.y;
            var cx = c2.x - p.x;
            var cy = c2.y - p.y;

            return (
                (ax * ax + ay * ay) * (bx * cy - cx * by) -
                (bx * bx + by * by) * (ax * cy - cx * ay) +
                (cx * cx + cy * cy) * (ax * by - bx * ay)
            ) > 0.000001f;
        }

        /// <summary>
        /// Rotate vector v left 90 degrees
        /// </summary>
        public static float2 RotateRightAngle(float2 v)
        {
            var x = v.x;
            v.x = -v.y;
            v.y = x;

            return v;
        }

        /// <summary>
        /// General line/line intersection method. Each line is defined by a
        /// two vectors, a point on the line (p0 and p1 for the two lines) and a
        /// direction vector (v0 and v1 for the two lines). The returned value
        /// indicates whether the lines intersect. m0 and m1 are the
        /// coefficients of how much you have to multiply the direction vectors
        /// to get to the intersection. 
        ///
        /// In other words, if the intersection is located at X, then: 
        ///
        ///     X = p0 + m0 * v0
        ///     X = p1 + m1 * v1
        ///
        /// By checking the m0/m1 values, you can check intersections for line
        /// segments and rays.
        /// </summary>
        public static bool LineLineIntersection(float2 p0, float2 v0, float2 p1, float2 v1, out float m0,
            out float m1)
        {
            var det = (v0.x * v1.y - v0.y * v1.x);

            if (math.abs(det) < 0.001f)
            {
                m0 = float.NaN;
                m1 = float.NaN;

                return false;
            }
            else
            {
                m0 = ((p0.y - p1.y) * v1.x - (p0.x - p1.x) * v1.y) / det;

                if (math.abs(v1.x) >= 0.001f)
                {
                    m1 = (p0.x + m0 * v0.x - p1.x) / v1.x;
                }
                else
                {
                    m1 = (p0.y + m0 * v0.y - p1.y) / v1.y;
                }

                return true;
            }
        }

        /// <summary>
        /// Returns the intersections of two lines. p0/p1 are points on the
        /// line, v0/v1 are the direction vectors for the lines. 
        ///
        /// If there are no intersections, returns a NaN vector
        /// <summary>
        public static float2 LineLineIntersection(float2 p0, float2 v0, float2 p1, float2 v1)
        {
            float m0, m1;

            if (LineLineIntersection(p0, v0, p1, v1, out m0, out m1))
            {
                return p0 + m0 * v0;
            }
            else
            {
                return new float2(float.NaN, float.NaN);
            }
        }

        /// <summary>
        /// Returns the center of the circumcircle defined by three points (c0,
        /// c1 and c2) on its edge.
        /// </summary>
        public static float2 CircumcircleCenter(float2 c0, float2 c1, float2 c2)
        {
            var mp0 = 0.5f * (c0 + c1);
            var mp1 = 0.5f * (c1 + c2);

            var v0 = RotateRightAngle(c0 - c1);
            var v1 = RotateRightAngle(c1 - c2);

            float m0, m1;

            Geom.LineLineIntersection(mp0, v0, mp1, v1, out m0, out m1);

            return mp0 + m0 * v0;
        }

        /// <summary>
        /// Returns the triangle centroid for triangle defined by points c0, c1
        /// and c2. 
        /// </summary>
        public static float2 TriangleCentroid(float2 c0, float2 c1, float2 c2)
        {
            var val = (1.0f / 3.0f) * (c0 + c1 + c2);
            return val;
        }

        /// <summary>
        /// Returns the signed area of a polygon. CCW polygons return a positive
        /// area, CW polygons return a negative area.
        /// </summary>
        public static float Area(IList<float2> polygon)
        {
            var area = 0.0f;

            var count = polygon.Count;

            for (int i = 0; i < count; i++)
            {
                var j = (i == count - 1) ? 0 : (i + 1);

                var p0 = polygon[i];
                var p1 = polygon[j];

                area += p0.x * p1.y - p1.y * p1.x;
            }

            return 0.5f * area;
        }

        public static uint Seed = 1;

        public static float NormalizedRandom(float mean, float stddev)
        {
            var r = Random.CreateFromIndex((uint)Seed++);
            var u1 = r.NextFloat();
            var u2 = r.NextFloat();

            var randStdNormal = math.sqrt(-2.0f * math.log(u1)) * math.sin(2.0f * math.PI * u2);

            return mean + stddev * randStdNormal;
        }

        public static float2[] RandomSite(float2 position, int sites)
        {
            var ret = new float2[sites];
            for (int i = 0; i < sites; i++)
            {
                var dist = math.abs(NormalizedRandom(0.5f, 1.0f / 2.0f));
                var angle = 2.0f * math.PI * Random.CreateFromIndex((uint)Seed++).NextFloat();
                ret[i] = position + new float2(
                    dist * math.cos(angle),
                    dist * math.sin(angle));
            }

            return ret;
        }
    }
}