using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    public LayerMask unwalkableMask;
    public Vector2 gridWorldSize;
    public float nodeRadius;
    Node[,] grid;//note, all arrays initialized this way are rectangular.

    float nodeDiameter;
    int gridSizeX, gridSizeY;

    [HideInInspector]
    public List<Node> path;

    private void Start()
    {
        nodeDiameter = nodeRadius * 2;
        gridSizeX = Mathf.RoundToInt(gridWorldSize.x / nodeDiameter);
        gridSizeY = Mathf.RoundToInt(gridWorldSize.y / nodeDiameter);
        CreateGrid();
    }


    //generates grid of nodes
    void CreateGrid()
    {
        grid = new Node[gridSizeX, gridSizeY];
        Vector3 worldBottomLeft = transform.position - Vector3.right * gridWorldSize.x / 2 - Vector3.forward * gridWorldSize.y / 2;

        for (int x = 0; x < gridSizeX; x++)
        {
            for (int y = 0; y < gridSizeY; y++)
            {

                Vector3 worldPoint = worldBottomLeft + Vector3.right * (x * nodeDiameter + nodeRadius) + Vector3.forward * (y * nodeDiameter + nodeRadius);
                bool walkable = !(Physics.CheckSphere(worldPoint, nodeRadius, unwalkableMask));
                grid[x, y] = new Node(walkable, worldPoint, x, y);

            }
        }

    }

    //returns a list of nodes that share an edge or a corner with given node
    public List<Node> GetNeighbors(Node node)
    {
        List<Node> output = new List<Node>();

        for(int x = -1; x <= 1; x++)
        {
            for (int y = -1; y <= 1; y++)
            {
                if (x == 0 && y == 0)
                    continue;
                int currX = node.gridX + x;
                int currY = node.gridY + y;

                if ((currX+gridSizeX)%gridSizeX == currX && (currY+gridSizeY)%gridSizeY == currY)
                {
                    output.Add(grid[currX, currY]);
                }
            }
        }
        return output;
    }


    //returns the node that a given world position is in
    public Node NodeFromWorldPoint(Vector3 worldPos)
    {
        float percentX = Mathf.Clamp01((worldPos.x + gridWorldSize.x / 2) / gridWorldSize.x);
        float percentY = Mathf.Clamp01((worldPos.z + gridWorldSize.y / 2) / gridWorldSize.y);

        int x = Mathf.RoundToInt((gridSizeX - 1) * percentX);
        int y = Mathf.RoundToInt((gridSizeY - 1) * percentY);

        return grid[x, y];

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(gridWorldSize.x, 1, gridWorldSize.y));//outline of grid area

        if (grid != null)
        {
            foreach(Node n in grid)
            {
                Gizmos.color = n.walkable ? Color.white : Color.black;//if node is an obstical color is red, otherwise color is white

                //if node is part of A* path, color is green
                if(path!=null && path.Contains(n))
                {
                    Gizmos.color = Color.green;
                }
                Gizmos.DrawCube(n.worldPosition, Vector3.one * (nodeDiameter - nodeRadius/5));//draw node
            }
        }

    }

}
