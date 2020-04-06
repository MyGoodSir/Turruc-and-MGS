using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Polygon
{

    public readonly Vector2[] points;
    public readonly int numPoints, numHullPoints, numHoles;
    public readonly int[] numPointsPerHole;
    readonly int[] holeStartIndices;

    public Polygon(Vector2[] hull, Vector2[][] holes)
    {
        numHullPoints = hull.Length;
        numHoles = holes.GetLength(0);

        numPointsPerHole = new int[numHoles];
        holeStartIndices = new int[numHoles];

        int numHolePointsSum = 0;

        for(int i = 0; i < holes.GetLength(0); i++)
        {
            numPointsPerHole[i] = holes[i].Length;

            holeStartIndices[i] = numHullPoints + numHolePointsSum;
            numHolePointsSum += numPointsPerHole[i];
        }

        numPoints = numHullPoints + numHolePointsSum;
        points = new Vector2[numPoints];


        //add hull points, CCW (Right Hand Rule)
        bool reverseHullPointsOrder = !PointsAreCounterClockwise(hull);
        for(int i =0; i < numHullPoints; i++)
        {
            points[i] = hull[reverseHullPointsOrder ? numHullPoints - i - 1 : i];
        }

        //add hole points, CW (Left Hand Rule)
        for(int i = 0; i < numHoles; i++)
        {
            bool reverseHolePointsOrder = PointsAreCounterClockwise(holes[i]);
            for(int j = 0; j < holes[i].Length; j++)
            {
                points[IndexOfPointInHole(j, i)] = holes[i][reverseHolePointsOrder ? holes[i].Length - j - 1 : j];

            }
        }
    }
    public Polygon(Vector2[] hull) : this(hull, new Vector2[0][])
    {
        
    }

    bool PointsAreCounterClockwise(Vector2[] test)
    {
        float signedArea = 0;
        for(int i =0; i < test.Length; i++)
        {
            int next = (i + 1) % test.Length;
            signedArea += (test[next].x - test[i].x) * (test[next].y + test[i].y);
        }
        return signedArea < 0;
    }

    public int IndexOfFirstPointInHole(int holeIndex)
    {
        return holeStartIndices[holeIndex];
    }
    public int IndexOfPointInHole(int index, int holeIndex)
    {
        return holeStartIndices[holeIndex] + index;
    }
    public Vector2 GetHolePoint(int index, int holeIndex)
    {
        return points[holeStartIndices[holeIndex] + index];
    }


}
