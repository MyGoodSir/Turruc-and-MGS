using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public partial class CompositeShape
{
    public class CompositeShapeData
    {
        public readonly Vector2[] points;
        public readonly Polygon poly;
        public readonly int[] tris;

        public List<CompositeShapeData> parents = new List<CompositeShapeData>();
        public List<CompositeShapeData> holes = new List<CompositeShapeData>();
        public bool IsValidShape { get; private set; }

        public CompositeShapeData(Vector3[] points)
        {
            this.points = points.Select(v => new Vector2(v.x, v.z)).ToArray();
            IsValidShape = points.Length >= 3 && !SelfIntersects();

            if (IsValidShape)
            {
                poly = new Polygon(this.points);
                Triangulator t = new Triangulator(poly);
                tris = t.Triangulate();
            }
        }

        //remove holes that overlap with another hole
        public void ValidateHoles()
        {
            for(int i = 0; i < holes.Count; i++)
            {
                for (int j = i + 1; j < holes.Count; j++)
                {
                    bool overlap = holes[i].OverlapsPartially(holes[j]);

                    if(overlap)
                    {
                        holes[i].IsValidShape = false;
                        break;
                    }
                }
            }
            for(int i = holes.Count - 1; i >= 0; i--)
            {
                if (!holes[i].IsValidShape)
                {
                    holes.RemoveAt(i);
                }
            }
        }

        //checks if some other shape exists entirely within this shape
        public bool IsParentOf(CompositeShapeData otherShape)
        {
            if (otherShape.parents.Contains(this))
            {
                return true;
            }
            if (parents.Contains(otherShape))
            {
                return false;
            }

            //check if the first point of "otherShape" is in this shape. 
            //if it is, that must mean that these shapes overlap somehow.
            bool pointInside = false;
            for (int i = 0; i < tris.Length; i += 3)
            {
                if (MGSUtil.MG2DMath.PointInTriangle(poly.points[tris[i]], poly.points[tris[i + 1]], poly.points[tris[i + 2]], otherShape.points[0]))
                {
                    pointInside = true;
                    break;
                }
            }

            //if the first point is not inside, then these two shapes can only partially overlap at best
            if (!pointInside)
            {
                return false;
            }

            //if these shapes don't partially overlap, but the first point of otherShape is inside of this shape,
            //then that must mean this shape completely contains otherShape 
            return !OverlapsPartially(otherShape);
        }

        //check if these two shapes partially overlap.
        public bool OverlapsPartially(CompositeShapeData otherShape)
        {
            //Check for intersections between this shape's line segments and otherShape's line segments
            //if one exists then these two shapes overlap partially, but not completely.
            for (int i = 0; i < points.Length; i++)
            {
                LineSegment s1 = new LineSegment(points[i], points[(i + 1) % points.Length]);

                for (int j = 0; j < otherShape.points.Length; j++)
                {
                    LineSegment s2 = new LineSegment(otherShape.points[j], otherShape.points[(j + 1) % otherShape.points.Length]);
                    if (MGSUtil.MG2DMath.LineSegmentsIntersect(s1.a, s1.b, s2.a, s2.b))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        //check if any of this shape's line segments intersect eachother
        public bool SelfIntersects()
        {
            for(int i = 0; i < points.Length; i++)
            {
                LineSegment s1 = new LineSegment(points[i], points[(i + 1) % points.Length]);
                for (int j = i+2; j < points.Length; j++)
                {
                    if((j+1)%points.Length == i)
                    {
                        continue;
                    }
                    LineSegment s2 = new LineSegment(points[j], points[(j + 1) % points.Length]);
                    if (MGSUtil.MG2DMath.LineSegmentsIntersect(s1.a, s1.b, s2.a, s2.b))
                    {
                        return true;
                    }

                }
            }
            return false;
        }

        public struct LineSegment
        {
            public readonly Vector2 a;
            public readonly Vector2 b;

            public LineSegment(Vector2 a, Vector2 b)
            {
                this.a = a;
                this.b = b;
            }
        }


    }
}
