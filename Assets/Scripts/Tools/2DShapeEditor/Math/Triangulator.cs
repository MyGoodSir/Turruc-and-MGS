using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Triangulator
{
    LinkedList<Vertex> clippedPolyVerts;
    int[] tris;
    int triIndex;


    public Triangulator(Polygon poly)
    {
        int numConnections = 2 * poly.numHoles;//number of hole to hull connection verts. 2 verts added each time a hole is connected to the hull
        int numVerts = poly.numPoints + numConnections;
        tris = new int[(numVerts - 2) * 3];
        clippedPolyVerts = GenerateVertexList(poly);

    }

    public int[] Triangulate()
    {
        while(clippedPolyVerts.Count>=3)
        {
            bool trimmedThisIter = false;//has an ear been removed during this iteration yet
            LinkedListNode<Vertex> vertNode = clippedPolyVerts.First;
            for(int i = 0; i < clippedPolyVerts.Count; i++)
            {
                //note: "??" is the null-coalescing operator. returns the left value if the left value is not null, otherwise it returns the right operator.
                //"??=" is the assignment operator version of that.
                LinkedListNode<Vertex> prevNode = vertNode.Previous ?? clippedPolyVerts.Last;
                LinkedListNode<Vertex> nextNode = vertNode.Next ?? clippedPolyVerts.First;

                if (vertNode.Value.isConvex)
                {
                    if(!TriangleContainsVertex(prevNode.Value, vertNode.Value, nextNode.Value))
                    {
                        //check if trimming ear makes a previously reflex vertex convex
                        if (!prevNode.Value.isConvex)
                        {
                            LinkedListNode<Vertex> prepre = prevNode.Previous ?? clippedPolyVerts.Last;

                            prevNode.Value.isConvex = IsConvex(prepre.Value.position, prevNode.Value.position, nextNode.Value.position);
                        }
                        if (!nextNode.Value.isConvex)
                        {
                            LinkedListNode<Vertex> postpost = nextNode.Next ?? clippedPolyVerts.First;

                            nextNode.Value.isConvex = IsConvex(prevNode.Value.position, nextNode.Value.position, postpost.Value.position);
                        }

                        //add triangle to array
                        tris[triIndex * 3 + 2] = prevNode.Value.index;
                        tris[triIndex * 3 + 1] = vertNode.Value.index;
                        tris[triIndex * 3] = nextNode.Value.index;
                        triIndex++;

                        trimmedThisIter = true;
                        clippedPolyVerts.Remove(vertNode);
                        break;

                    }
                }

                vertNode = nextNode;

            }

            if(!trimmedThisIter)
            {
                Debug.LogError("Triangulator broke D:");
                return null;
            }

        }
        return tris;
    }



    LinkedList<Vertex> GenerateVertexList(Polygon poly)
    {
        LinkedList<Vertex> vlist = new LinkedList<Vertex>();
        LinkedListNode<Vertex> curr = null;//current node

        //Add all hull points to list
        for(int i = 0; i < poly.numHullPoints; i++)
        {
            int prevIndex = (i - 1 + poly.numHullPoints) % poly.numHullPoints;
            int nextIndex = (i + 1) % poly.numHullPoints;

            bool vertConvex = IsConvex(poly.points[prevIndex], poly.points[i], poly.points[nextIndex]);
            Vertex currHullVert = new Vertex(poly.points[i], i, vertConvex);

            if(curr == null)
            {
                curr = vlist.AddFirst(currHullVert);
            }
            else
            {
                curr = vlist.AddAfter(curr, currHullVert);
            }
        }

        //process holes ;)
        List<HoleData> sortedHoleData = new List<HoleData>();

        for(int i = 0; i<poly.numHoles; i++)
        {
            //rightmost point in hole will be the bridge point where the hole is connected to the hull
            Vector2 bp = new Vector2(float.MinValue, 0);//bp stands for brisge point
            int bpIndex = 0;
            for(int j = 0; j<poly.numPointsPerHole[i]; j++)
            {
                if(poly.GetHolePoint(j,i).x > bp.x)
                {
                    bp = poly.GetHolePoint(j, i);
                    bpIndex = j;
                }
            }
            sortedHoleData.Add(new HoleData(i, bpIndex, bp));
        }

        //sort hole data from rightmost to leftmost
        sortedHoleData.Sort((x, y) => (x.bridgePoint.x > y.bridgePoint.x) ? -1 : 1);

        foreach(HoleData data in sortedHoleData)
        {

            //find first edge directly right of bridge point
            Vector2 rayIntersectPoint = new Vector2(float.MaxValue, data.bridgePoint.y);
            List<LinkedListNode<Vertex>> hullNodesPotentiallyInBridgeTriangle = new List<LinkedListNode<Vertex>>();
            LinkedListNode<Vertex> initialBridgeNodeOnHull = null;
            curr = vlist.First;
            while (curr != null)
            {
                LinkedListNode<Vertex> next = (curr.Next == null) ? vlist.First : curr.Next;
                Vector2 p0 = curr.Value.position;
                Vector2 p1 = next.Value.position;

                //for ray intersection, at least one point must be to the right of bridge point
                if(p0.x>data.bridgePoint.x || p1.x > data.bridgePoint.x)
                {
                    //p0 is above bridge point XOR p1 is above bridge point
                    if(p0.y > data.bridgePoint.y != p1.y > data.bridgePoint.y)
                    {
                        float rayIntersectX = p1.x; //only if line from p0 to p1 is vertical
                        if(!Mathf.Approximately(p0.x, p1.x))
                        {
                            float intersectY = data.bridgePoint.y;
                            float gradient = (p0.y - p1.y) / (p0.x - p1.x);
                            float c = p1.y - gradient * p1.x;
                            rayIntersectX = (intersectY - c) / gradient;
                        }

                        //intersection must be right of bridge point
                        if (rayIntersectX > data.bridgePoint.x)
                        {
                            LinkedListNode<Vertex> potentialNewBridgeNode = (p0.x > p1.x) ? curr : next;
                            //duplicate edge is an edge shared by a hole and it's parent poly. 2 intersections will occur at a duplicate edge
                            bool isDuplicateEdge = Mathf.Approximately(rayIntersectX, rayIntersectPoint.x);
                            bool connectToThisDuplicateEdge = data.bridgePoint.y > potentialNewBridgeNode.Previous.Value.position.y;

                            if(!isDuplicateEdge || connectToThisDuplicateEdge)
                            {
                                //if this is the closest intersection so far, set bridge hull node to the point in the line with the greatest X value
                                if (rayIntersectX < rayIntersectPoint.x || isDuplicateEdge)
                                {
                                    rayIntersectPoint.x = rayIntersectX;
                                    initialBridgeNodeOnHull = potentialNewBridgeNode;
                                }

                            }
                        }
                    }
                }

                //determine if current node might be in the triangle formed by holeBrisgePoint, rayIntersection, and bridgeNodeOnHull
                //only consider reflex nodes
                if (curr != initialBridgeNodeOnHull)
                {
                    if(!curr.Value.isConvex && p0.x > data.bridgePoint.x)
                    {
                        hullNodesPotentiallyInBridgeTriangle.Add(curr);
                    }
                }
                curr = curr.Next;

            }

            //check for points in bridge triangle
            //if any exist, have a battle royale for one to become the new bridgeNodeOnHull
            LinkedListNode<Vertex> validBridgeNodeOnHull = initialBridgeNodeOnHull;
            foreach(LinkedListNode<Vertex> node in hullNodesPotentiallyInBridgeTriangle)
            {
                if(node.Value.index == initialBridgeNodeOnHull.Value.index)
                {
                    continue;
                }
                //if there is a point in the triangle, current bridgeNodeOnHull is invalid
                if(MGSUtil.MG2DMath.PointInTriangle(data.bridgePoint, rayIntersectPoint, initialBridgeNodeOnHull.Value.position, node.Value.position))
                {
                    bool isDuplicatePoint = validBridgeNodeOnHull.Value.position == node.Value.position;

                    //choose node with smallest angle from holeBridgeNode
                    //if point is duplicate, use whichever clone is closest to the end of the list
                    float currYDstFromHoleBridge = Mathf.Abs(data.bridgePoint.y - validBridgeNodeOnHull.Value.position.y);
                    float triPntYDstFromHoleBridge = Mathf.Abs(data.bridgePoint.y - node.Value.position.y);

                    if(triPntYDstFromHoleBridge < currYDstFromHoleBridge || isDuplicatePoint)
                    {
                        validBridgeNodeOnHull = node;
                    }
                }
            }

            //insert hole points, starting from bridge node, intro vertex list at validBridgeNodeOnHull
            curr = validBridgeNodeOnHull;
            for(int i = data.bridgeIndex; i<=poly.numPointsPerHole[data.holeIndex] + data.bridgeIndex; i++)
            {
                int prev = curr.Value.index;
                int index = poly.IndexOfPointInHole(i % poly.numPointsPerHole[data.holeIndex], data.holeIndex);
                int next = poly.IndexOfPointInHole((i + 1) % poly.numPointsPerHole[data.holeIndex], data.holeIndex);

                if(i==poly.numPointsPerHole[data.holeIndex] + data.bridgeIndex)//returned to start
                {
                    next = validBridgeNodeOnHull.Value.index; //next point is the point on the hull
                }

                bool vertConvex = IsConvex(poly.points[prev], poly.points[index], poly.points[next]);
                Vertex holeVert = new Vertex(poly.points[index], index, vertConvex);
                curr = vlist.AddAfter(curr, holeVert);
            }

            //Add duplicate hull bridge vert and set concavity
            Vector2 nextPos = (curr.Next == null) ? vlist.First.Value.position : curr.Next.Value.position;
            bool convex = IsConvex(data.bridgePoint, validBridgeNodeOnHull.Value.position, nextPos);
            Vertex dupeVert = new Vertex(validBridgeNodeOnHull.Value.position, validBridgeNodeOnHull.Value.index, convex);
            vlist.AddAfter(curr, dupeVert);

            //Set concavity of initial hull bridge vert bc it may have changed
            LinkedListNode<Vertex> prevNode = (validBridgeNodeOnHull.Previous == null) ? vlist.Last : validBridgeNodeOnHull.Previous;
            LinkedListNode<Vertex> nextNode = (validBridgeNodeOnHull.Next == null) ? vlist.First : validBridgeNodeOnHull.Next;
            validBridgeNodeOnHull.Value.isConvex = IsConvex(prevNode.Value.position, validBridgeNodeOnHull.Value.position, nextNode.Value.position);

        }
        return vlist;
    }





    
    bool TriangleContainsVertex(Vertex v0, Vertex v1, Vertex v2)
    {
        LinkedListNode<Vertex> vertNode = clippedPolyVerts.First;
        for(int i =0; i < clippedPolyVerts.Count; i++)
        {
            if(!vertNode.Value.isConvex)
            {
                Vertex toCheck = vertNode.Value;
                if (toCheck.index != v0.index && toCheck.index != v1.index && toCheck.index != v2.index)
                {
                    if(MGSUtil.MG2DMath.PointInTriangle(v0.position, v1.position, v2.position, toCheck.position))
                    {
                        return true;
                    }
                }
            }
            vertNode = vertNode.Next;
        }
        return false;
    }
    //v1 is convex if the points v0, v1, v2 are in counter-clockwise order 
    bool IsConvex(Vector2 v0, Vector2 v1, Vector2 v2)
    {
        return MGSUtil.MG2DMath.SideOfLine(v0, v2, v1) == -1;
    }

    public struct HoleData
    {
        public readonly int holeIndex;
        public readonly int bridgeIndex;
        public readonly Vector2 bridgePoint;

        public HoleData(int holeIndex, int bridgeIndex, Vector2 bridgePoint)
        {
            this.holeIndex = holeIndex;
            this.bridgeIndex = bridgeIndex;
            this.bridgePoint = bridgePoint;
        }
    }

    public class Vertex
    {
        public readonly Vector2 position;
        public readonly int index;
        public bool isConvex;

        public Vertex(Vector2 position, int index, bool isConvex)
        {
            this.position = position;
            this.index = index;
            this.isConvex = isConvex;
        }
    }
}
