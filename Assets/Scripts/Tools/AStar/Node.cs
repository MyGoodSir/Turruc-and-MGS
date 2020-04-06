using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gCost, hCost;//hCost = distanceCost from this node to end node :::: gCost = accumulated cost from start to this node
    public int gridX, gridY;
    public Node parent;

    public Node(bool walkable, Vector3 worldPosition, int gridX, int gridY)
    {
        this.walkable = walkable;
        this.worldPosition = worldPosition;
        this.gridX = gridX;
        this.gridY = gridY;
    }
    public int fCost
    {
        get
        {
            return gCost + hCost;
        }
    }

    //distance from this node to a specified node
    public int GetDistance(Node other)
    {
        int x = Mathf.Abs(gridX - other.gridX);
        int y = Mathf.Abs(gridY - other.gridY);

        return 14 * Mathf.Min(x, y) + 10 * Mathf.Abs(x - y);

    }
}
