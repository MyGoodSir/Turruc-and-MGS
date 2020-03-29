using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public partial class CompositeShape
{
    public Vector3[] verts;//vertices
    public int[] tris;//triangles

    Shape[] shapes;
    float height = 0;

    public CompositeShape(IEnumerable<Shape> shapes)
    {
        this.shapes = shapes.ToArray();
    }

    public Mesh GetMesh()
    {
        Process();

        return new Mesh()
        {
            vertices = verts,
            triangles = tris,
            normals = verts.Select(x => Vector3.up).ToArray()
        };
        
    }
    public void Process()
    {
        //generates array of shape data and makes sure it's valid
        CompositeShapeData[] validShapes = shapes.Select(x => new CompositeShapeData(x.points.ToArray())).Where(x => x.IsValidShape).ToArray();

        //parent shapes completely contain their child shapes
        for(int i = 0; i < validShapes.Length; i++)
        {
            for(int j = 0; j < validShapes.Length; j++)
            {
                if (i == j)
                    continue;

                if (validShapes[i].IsParentOf(validShapes[j]))
                {
                    validShapes[j].parents.Add(validShapes[i]);
                }
            }
        }

        //"hole" refers to a shape that has an odd number of parents
        CompositeShapeData[] holes = validShapes.Where(x => x.parents.Count % 2 != 0).ToArray();
        foreach(CompositeShapeData hole in holes)
        {
            //direct parent is the smallest shape surrounding the child shape, meaning it's the parent with the largest number of parent shapes
            CompositeShapeData directParent = hole.parents.OrderByDescending(x => x.parents.Count).First();
            directParent.holes.Add(hole);
        }

        CompositeShapeData[] solidShapes = validShapes.Where(x => x.parents.Count % 2 == 0).ToArray();

        foreach(CompositeShapeData shape in solidShapes)
        {
            shape.ValidateHoles();
        }

        Polygon[] polys = solidShapes.Select(x => new Polygon(x.poly.points, x.holes.Select(h => h.poly.points).ToArray())).ToArray();

        verts = polys.SelectMany(x => x.points.Select(vec2 => new Vector3(vec2.x, height, vec2.y))).ToArray();

        List<int> allTris = new List<int>();
        int startIndex = 0;
        for(int i = 0; i<polys.Length; i++)
        {
            Triangulator tlr = new Triangulator(polys[i]);
            int[] polyTris = tlr.Triangulate();

            for(int j = 0; j < polyTris.Length; j++)
            {
                allTris.Add(polyTris[j] + startIndex);
            }
            startIndex += polys[i].numPoints;
        }

        tris = allTris.ToArray();

    }


}
