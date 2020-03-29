using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class MGSUtil
{
    public static class MG2DMath
    {

        /*
         * 
         * TODO: Rewrite functions as original.
         * 
         * 
         * Code Below is written by Sebasistian Lague
         * 
         */
        public static float PseudoDistanceFromPointToLine(Vector2 a, Vector2 b, Vector2 c)
        {
            return Mathf.Abs((c.x - a.x) * (-b.y + a.y) + (c.y - a.y) * (b.x - a.x));
        }

        public static int SideOfLine(Vector2 a, Vector2 b, Vector2 c)
        {
            //a and b are the ends of a line segment, we return what side of that line segment is facing c
            return (int)Mathf.Sign((c.x - a.x) * (-b.y + a.y) + (c.y - a.y) * (b.x - a.x));
        }

        public static int SideOfLine(float ax, float ay, float bx, float by, float cx, float cy)
        {
            return (int)Mathf.Sign((cx - ax) * (-by + ay) + (cy - ay) * (bx - ax));
        }

        public static bool PointInTriangle(Vector2 a, Vector2 b, Vector2 c, Vector2 p)
        {
            float area = 0.5f * (-b.y * c.x + a.y * (-b.x + c.x) + a.x * (b.y - c.y) + b.x * c.y);
            float s = 1 / (2 * area) * (a.y * c.x - a.x * c.y + (c.y - a.y) * p.x + (a.x - c.x) * p.y);
            float t = 1 / (2 * area) * (a.x * b.y - a.y * b.x + (a.y - b.y) * p.x + (b.x - a.x) * p.y);
            return s >= 0 && t >= 0 && (s + t) <= 1;

        }

        public static bool LineSegmentsIntersect(Vector2 a1, Vector2 b1, Vector2 a2, Vector2 b2)
        {
            float denominator = ((b1.x - a1.x) * (b2.y - a2.y)) - ((b1.y - a1.y) * (b2.x - a2.x));
            if (Mathf.Approximately(denominator, 0))
            {
                return false;
            }

            float numerator1 = ((a1.y - a2.y) * (b2.x - a2.x)) - ((a1.x - a2.x) * (b2.y - a2.y));
            float numerator2 = ((a1.y - a2.y) * (b1.x - a1.x)) - ((a1.x - a2.x) * (b1.y - a1.y));

            if (Mathf.Approximately(numerator1, 0) || Mathf.Approximately(numerator2, 0))
            {
                return false;
            }

            float r = numerator1 / denominator;
            float s = numerator2 / denominator;

            return (r > 0 && r < 1) && (s > 0 && s < 1);
        }
    }
}