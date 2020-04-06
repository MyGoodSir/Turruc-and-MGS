using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pathfinding : MonoBehaviour
{
    public Transform seeker, target;

    Grid grid;

    private void Awake()
    {
        grid = GetComponent<Grid>();//grabs grid component of parent object
    }
    private void Update()
    {
        FindPath(seeker.position, target.position);
    }

    //A* algorithm
    void FindPath(Vector3 startPos, Vector3 endPos)
    {
        Node start = grid.NodeFromWorldPoint(startPos);
        Node end = grid.NodeFromWorldPoint(endPos);

        List<Node> open = new List<Node>();
        HashSet<Node> closed = new HashSet<Node>();

        open.Add(start);

        while (open.Count > 0)
        {
            Node current = open[0];
            for(int i = 1; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost || (open[i].fCost == current.fCost && open[i].hCost < current.hCost))
                {
                    current = open[i];
                }
            }
            open.Remove(current);
            closed.Add(current);

            if(current == end)
            {
                Retrace(start, end);
                return;
            }

            foreach(Node n in grid.GetNeighbors(current))
            {
                if (!n.walkable || closed.Contains(n))
                    continue;

                int newDist = current.gCost + current.GetDistance(n);
                if(newDist < n.gCost || !open.Contains(n))
                {
                    n.gCost = newDist;
                    n.hCost = n.GetDistance(end);
                    n.parent = current;

                    if (!open.Contains(n))
                    {
                        open.Add(n);
                    }
                }
            }

        }
    }

    //traces path from end back to start
    void Retrace(Node start, Node end)
    {
        List<Node> path = new List<Node>();
        Node current = end;

        while(current != start)
        {
            path.Add(current);
            current = current.parent;
        }
        path.Reverse();

        grid.path = path;

    }


}
