using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;


namespace WertheApp.RN
{

    /**********************************************************************
    *********************************************************************/
    public class DijkstraAlgorithm
    {
        //VARIABLES
        private static String[,] tableValues;
        private static bool[,] visitedEdges;
        private static DNetwork network;
        private static String[] n;
        private static bool[] uv, ux, uw, uy, zw, zy, zv, zx, vx, vy, vw, xw, xy, yw;
        private static String[] forwarding;

        private static String[] dv, dw, dx, dy, dz;

        //CONSTRUCTOR
        public DijkstraAlgorithm(String[] a)
        {
            //only needed when creating an object
        }

        //METHODS
        private static void CreateForwardingTable()
        {
            forwarding = new String[5];
            int count = -1; 
            foreach(DNode node in network.nodesList)
            {
              
                if(count >= 0)
                {
                   forwarding[count] = GetForwarding(node, network.nodesList.First());
                }
                count++;
            }
            
        }

        public static String[] GetForwardingTable()
        {
            return forwarding;
        }

        public static void CreateVisitedEdges()
        {

           visitedEdges = new bool[32, 14];

            for(int i = 0; i < 32; i++)
            {
                int j = 0;
                if (i > 5){ j = 1; }
                if (i > 11) { j = 2; }
                if (i > 17) { j = 3; }
                if (i > 23) { j = 4; }
                if (i > 29) { j = 5; }
                visitedEdges[i, 0] = uv[j];
                visitedEdges[i, 1] = ux[j];
                visitedEdges[i, 2] = uw[j];
                visitedEdges[i, 3] = uy[j];
                visitedEdges[i, 4] = zw[j];
                visitedEdges[i, 5] = zy[j];
                visitedEdges[i, 6] = zv[j];
                visitedEdges[i, 7] = zx[j];
                visitedEdges[i, 8] = vx[j];
                visitedEdges[i, 9] = vy[j];
                visitedEdges[i, 10] = vw[j];
                visitedEdges[i, 11] = xw[j];
                visitedEdges[i, 12] = xy[j];
                visitedEdges[i, 13] = yw[j];
            }

        }

        public static bool[,] GetVisistedEdges()
        {
            return visitedEdges;
        }

        public static void CreateTableValuesArray()
        {
            //make N'
            n = new string[6];
            int index = 0;
            string s = "";
            foreach (DNode node in network.visitedNodesList)
            {
                String name = node.GetNodeName();
                s = s + name;
                n[index] = s;
                index++;
            }

            //∞
            //step, round(Dijkstra step), N', D(v), D(w), D(x), D(y), D(z)
            tableValues = new String[,] {
            { "0", "0", n[0] ,  "",        "",     "",     "",     "" , n[0]},
            { "1", "0", "",     dv[0] ,    "",     "",     "",     "" , n[0]},
            { "2", "0", "",     "",        dw[0],  "",     "",     "" , n[0]},
            { "3", "0", "",     "",        "",     dx[0],  "",     "" , n[0]},
            { "4", "0", "",     "",        "",     "",     dy[0],  "" , n[0]},
            { "5", "0", "",     "",        "",     "",     "",     dz[0] , n[0]},

            { "6",  "1", n[1],  "",        "",     "",     "",     "" , n[1]},
            { "7",  "1", "",    dv[1],     "",     "",     "",     "" , n[1]},
            { "8",  "1", "",    "",        dw[1],  "",     "",     "" , n[1]},
            { "9",  "1", "",    "",        "",     dx[1],  "",     "" , n[1]},
            { "10", "1", "",    "",        "",     "",     dy[1],  "" , n[1]},
            { "11", "1", "",    "",        "",     "",     "",     dz[1] , n[1]},

            { "12", "2", n[2],  "",        "",     "",     "",     "" , n[2]},
            { "13", "2", "",    dv[2],     "",     "",     "",     "" , n[2]},
            { "14", "2", "",    "",        dw[2],  "",     "",     "" , n[2]},
            { "15", "2", "",    "",        "",     dx[2],  "",     "" , n[2]},
            { "16", "2", "",    "",        "",     "",     dy[2],  "" , n[2]},
            { "17", "2", "",    "",        "",     "",     "",     dz[2] , n[2]},

            { "18", "3", n[3],  "",        "",     "",     "",     "" , n[3]},
            { "19", "3", "",    dv[3],     "",     "",     "",     "" , n[3]},
            { "20", "3", "",    "",        dw[3],  "",     "",     "" , n[3]},
            { "21", "3", "",    "",        "",     dx[3],  "",     "" , n[3]},
            { "22", "3", "",    "",        "",     "",     dy[3],  "" , n[3]},
            { "23", "3", "",    "",        "",     "",     "",     dz[3] , n[3]},

            { "24", "4", n[4],  "",        "",     "",     "",     "" , n[4]},
            { "25", "4", "",    dv[4],     "",     "",     "",     "" , n[4]},
            { "26", "4", "",    "",        dw[4],  "",     "",     "" , n[4]},
            { "27", "4", "",    "",        "",     dx[4],  "",     "" , n[4] },
            { "28", "4", "",    "",        "",     "",     dy[4],  "" , n[4] },
            { "29", "4", "",    "",        "",     "",     "",     dz[4] , n[4]},

            { "30", "5", n[5],  "",        "",     "",     "",     "" , n[5]},
            { "31", "5", "",  "",        "",     "",     "",     "" , n[5]}

            };
        }

        public static void Initialize()
        {
            //nodes
            String ph = "#"; 
            dv = new String[5] { ph, ph, ph, ph, ph };
            dw = new String[5] { ph, ph, ph, ph, ph };
            dx = new String[5] { ph, ph, ph, ph, ph };
            dy = new String[5] { ph, ph, ph, ph, ph };
            dz = new String[5] { ph, ph, ph, ph, ph };

            //edges
            uv = new bool[6];
            ux = new bool[6];
            uw = new bool[6];
            uy = new bool[6];
            zw = new bool[6];
            zy = new bool[6];
            zv = new bool[6];
            zx = new bool[6];
            vx = new bool[6];
            vy = new bool[6];
            vw = new bool[6];
            xw = new bool[6];
            xy = new bool[6];
            yw = new bool[6];

            int discovery = 0;
            //visit very first node (start node)
            network.VisitNode(network.nodesList.First(), discovery);

            //until all nodes are visited
            while(network.unvisitedNodesList.Count != 0)
            {
                AssignNewWeightsToNeighbors(network.visitedNodesList.Last());
                MakeArraysForTable(network.visitedNodesList.Last(), discovery);
                
                DNode nodeToVisitNext = FindUnvisitedNodeWithMinWeight();
                discovery++;
                network.VisitEdge(nodeToVisitNext.GetPreviousNode(), nodeToVisitNext);
                network.VisitNode(nodeToVisitNext, discovery);
                MakeArrayForGraph(discovery);

            }

            CreateForwardingTable();
        }

        public static void MakeArrayForGraph(int discovery)
        {
            foreach(DEdge edge in network.visitedEdgesList)
            {

                if (edge.Check("u", "v")) { uv[discovery] = true; };
                if (edge.Check("u", "x")) { ux[discovery] = true; };
                if (edge.Check("u", "w")) { uw[discovery] = true; };
                if (edge.Check("u", "y")) { uy[discovery] = true; };
                if (edge.Check("z", "w")) { zw[discovery] = true; };
                if (edge.Check("z", "y")) { zy[discovery] = true; };
                if (edge.Check("z", "v")) { zv[discovery] = true; };
                if (edge.Check("z", "x")) { zx[discovery] = true; };
                if (edge.Check("v", "x")) { vx[discovery] = true; };
                if (edge.Check("v", "y")) { vy[discovery] = true; };
                if (edge.Check("v", "w")) { vw[discovery] = true; };
                if (edge.Check("x", "w")) { xw[discovery] = true; };
                if (edge.Check("x", "y")) { xy[discovery] = true; };
                if (edge.Check("y", "w")) { yw[discovery] = true; };    
            }


        }

        public static void MakeArraysForTable(DNode node, int discovery)
        {
            foreach (DNode neighbor in node.GetNeighbors())
            {

                String name1 = neighbor.GetNodeName();
                String weight1 = neighbor.GetWeight().ToString();
                if (!neighbor.IsVisited())
                {
                    String prev = neighbor.GetPreviousNode().GetNodeName();
                    weight1 = weight1 + ", " + prev;
                }


                switch (name1)
                {
                    //case "u": break;
                    case "v": dv[discovery] = weight1; break;
                    case "w": dw[discovery] = weight1; break;
                    case "x": dx[discovery] = weight1; break;
                    case "y": dy[discovery] = weight1; break;
                    case "z": dz[discovery] = weight1; break;
                }
            }
          
            foreach (DNode nd in network.nodesList)
            {
                String name = nd.GetNodeName();
                String weight = nd.GetWeight().ToString();

                //unreachable nodes //∞
                if (weight == "999999999")
                {
                    weight = "∞";
                    switch (name)
                    {
                        //case "u": break;
                        case "v": dv[discovery] = weight; break;
                        case "w": dw[discovery] = weight; break;
                        case "x": dx[discovery] = weight; break;
                        case "y": dy[discovery] = weight; break;
                        case "z": dz[discovery] = weight; break;
                    }
                }

                //already visited nodes
                else if (nd.IsVisited()){
                    weight = "-";
                    switch (name)
                    {
                        //case "u": break;
                        case "v": dv[discovery] = weight; break;
                        case "w": dw[discovery] = weight; break;
                        case "x": dx[discovery] = weight; break;
                        case "y": dy[discovery] = weight; break;
                        case "z": dz[discovery] = weight; break;
                    }
                }

                //nodes that stay the same weight
                else 
                {
                    switch (name)
                    {
                        //case "u": break;
                        case "v":
                            if (dv[discovery] == "#")
                            {
                                dv[discovery] = dv[discovery-1];
                            }
                            break;
                        case "w":
                            if (dw[discovery] == "#")
                            {
                                dw[discovery] = dw[discovery-1];
                            }
                            break;
                        case "x":
                            if (dx[discovery] == "#")
                            {
                                dx[discovery] = dx[discovery-1];
                            }
                            break;
                        case "y":
                            if (dy[discovery] == "#")
                            {
                                dy[discovery] = dy[discovery-1];
                            }
                            break;
                        case "z":
                            if (dz[discovery] == "#")
                            {
                                dz[discovery] = dz[discovery-1];
                            }
                            break;
                    }
                }

            }

            /*
            //D(v), D(w), D(x), D(y), D(z)
            foreach (DNode neighbor in node.GetNeighbors())
            {
                Debug.WriteLine("TEST");
                String name = neighbor.GetNodeName();
                String weight = neighbor.GetWeight().ToString();
                if (neighbor.IsVisited())
                {
                    weight = "-"; 
                }
                else {
                    String prev = neighbor.GetPreviousNode().GetNodeName();
                    weight = weight + ", " + prev;
                }

                switch (name)
                {
                    //case "u": break;
                    case "v": dv[discovery] = weight; break;
                    case "w": dw[discovery] = weight; break;
                    case "x": dx[discovery] = weight; break;
                    case "y": dy[discovery] = weight; break;
                    case "z": dz[discovery] = weight; break;
                }
            }*/
        }

        public static void AssignNewWeightsToNeighbors(DNode node)
        {
            //network.visitedNodesList.Last()
            //find neighbors
            foreach (DNode neighbor in node.GetNeighbors())
            {
                if (!neighbor.IsVisited())
                {
                    
                    //assign combined weight of connecting edge and previous node
                    //to neighbor if it's smaller than the current weight of neighbor
                    int edgeWeight = GetWeightFromEdge(node, neighbor);
                    int prevWeight = node.GetWeight();
                    int weight = neighbor.GetWeight();
                    
                    if ((edgeWeight + prevWeight) < weight)
                    {
                        weight = (edgeWeight + prevWeight);
                        neighbor.AddPreviousNode(ref node);
                    }
                    neighbor.SetWeight(weight);
                }
            }
        }

        public static DNode FindUnvisitedNodeWithMinWeight()
        {
       
            //find unvisited neighbor of node with the minimal edge cost
            int minWeight = 999999999;
            DNode minNode = null;
            foreach (DNode node in network.unvisitedNodesList)
            {
                //find neighbor with minimal weight
                if (node.GetWeight() < minWeight)
                {
                    minWeight = node.GetWeight();
                    minNode = node;
                }
            }
            return minNode;

        }

        public static int GetWeightFromEdge(DNode a, DNode b)
        {

            foreach (DEdge edge in network.edgesList)
            {
                if(edge.GetNodeA() == a && edge.GetNodeB() == b
                    || edge.GetNodeA() == b && edge.GetNodeB() == a)
                {
                    return edge.GetWeight();
                }
            }
            return 0;
        }

        private static String GetForwarding(DNode endNode, DNode node)
        {
            String tracedDvpv = "";
            DNode curr = endNode;
            DNode prev = endNode.GetPreviousNode();

            if (node.GetIsStartNode())
            {
                while (prev != node)
                {
                    
                    curr = prev;
                    prev = curr.GetPreviousNode();


                }
                tracedDvpv = "(" + prev.GetNodeName() + "," + curr.GetNodeName() +")";
            }
            return tracedDvpv;
        }


        public static void BuildNetwork1(String[] a)
        {
            //define nodes with U as start node
            DNode nodeU = new DNode("u");
            DNode nodeV = new DNode("v");
            DNode nodeW = new DNode("w");
            DNode nodeX = new DNode("x");
            DNode nodeY = new DNode("y");
            DNode nodeZ = new DNode("z");
            nodeU.SetStartNode();

            //define network and add nodes to network
            network = new DNetwork(1);
            network.AddNode(ref nodeU);
            network.AddNode(ref nodeV);
            network.AddNode(ref nodeW);
            network.AddNode(ref nodeX);
            network.AddNode(ref nodeY);
            network.AddNode(ref nodeZ);

            //define edges and add to network
            DEdge edgeUV = new DEdge(ref nodeU, ref nodeV, Int32.Parse(a[0]));
            DEdge edgeUX = new DEdge(ref nodeU, ref nodeX, Int32.Parse(a[1]));
            DEdge edgeZW = new DEdge(ref nodeZ, ref nodeW, Int32.Parse(a[4]));
            DEdge edgeZY = new DEdge(ref nodeZ, ref nodeY, Int32.Parse(a[5]));
            DEdge edgeVY = new DEdge(ref nodeV, ref nodeY, Int32.Parse(a[9]));
            DEdge edgeVW = new DEdge(ref nodeV, ref nodeW, Int32.Parse(a[10]));
            DEdge edgeXW = new DEdge(ref nodeX, ref nodeW, Int32.Parse(a[11]));
            DEdge edgeXY = new DEdge(ref nodeX, ref nodeY, Int32.Parse(a[12]));
            network.AddEdge(ref edgeUV);
            network.AddEdge(ref edgeUX);
            network.AddEdge(ref edgeZW);
            network.AddEdge(ref edgeZY);
            network.AddEdge(ref edgeXY);
            network.AddEdge(ref edgeVW);
            network.AddEdge(ref edgeVY);
            network.AddEdge(ref edgeXW);

            

        }

        public static void BuildNetwork2(String[] a)
        {
            //define nodes with U as start node
            DNode nodeU = new DNode("u");
            DNode nodeV = new DNode("v");
            DNode nodeW = new DNode("w");
            DNode nodeX = new DNode("x");
            DNode nodeY = new DNode("y");
            DNode nodeZ = new DNode("z");
            nodeU.SetStartNode();

            //define network and add nodes to network
            network = new DNetwork(2);
            network.AddNode(ref nodeU);
            network.AddNode(ref nodeV);
            network.AddNode(ref nodeW);
            network.AddNode(ref nodeX);
            network.AddNode(ref nodeY);
            network.AddNode(ref nodeZ);

            //define edges and add to network
            DEdge edgeUV = new DEdge(ref nodeU, ref nodeV, Int32.Parse(a[0]));
            DEdge edgeUX = new DEdge(ref nodeU, ref nodeX, Int32.Parse(a[1]));
            DEdge edgeUW = new DEdge(ref nodeU, ref nodeW, Int32.Parse(a[2]));
            DEdge edgeZW = new DEdge(ref nodeZ, ref nodeW, Int32.Parse(a[4]));
            DEdge edgeZY = new DEdge(ref nodeZ, ref nodeY, Int32.Parse(a[5]));
            DEdge edgeVX = new DEdge(ref nodeV, ref nodeX, Int32.Parse(a[8]));
            DEdge edgeVW = new DEdge(ref nodeV, ref nodeW, Int32.Parse(a[10]));
            DEdge edgeXW = new DEdge(ref nodeX, ref nodeW, Int32.Parse(a[11]));
            DEdge edgeXY = new DEdge(ref nodeX, ref nodeY, Int32.Parse(a[12]));
            DEdge edgeYW = new DEdge(ref nodeY, ref nodeW, Int32.Parse(a[13]));

            network.AddEdge(ref edgeUV);
            network.AddEdge(ref edgeUX);
            network.AddEdge(ref edgeZW);
            network.AddEdge(ref edgeZY);
            network.AddEdge(ref edgeXY);
            network.AddEdge(ref edgeVW);
            network.AddEdge(ref edgeXW);
            network.AddEdge(ref edgeVX);
            network.AddEdge(ref edgeYW);
            network.AddEdge(ref edgeUW);
        }

        public static void BuildNetwork3(String[] a)
        {
            //define nodes with U as start node
            DNode nodeU = new DNode("u");
            DNode nodeV = new DNode("v");
            DNode nodeW = new DNode("w");
            DNode nodeX = new DNode("x");
            DNode nodeY = new DNode("y");
            DNode nodeZ = new DNode("z");
            nodeU.SetStartNode();

            //define network and add nodes to network
            network = new DNetwork(3);
            network.AddNode(ref nodeU);
            network.AddNode(ref nodeV);
            network.AddNode(ref nodeW);
            network.AddNode(ref nodeX);
            network.AddNode(ref nodeY);
            network.AddNode(ref nodeZ);

            //define edges and add to network
            DEdge edgeUV = new DEdge(ref nodeU, ref nodeV, Int32.Parse(a[0]));
            DEdge edgeUX = new DEdge(ref nodeU, ref nodeX, Int32.Parse(a[1]));
            DEdge edgeUY = new DEdge(ref nodeU, ref nodeY, Int32.Parse(a[3]));
            DEdge edgeZW = new DEdge(ref nodeZ, ref nodeW, Int32.Parse(a[4]));
            DEdge edgeZY = new DEdge(ref nodeZ, ref nodeY, Int32.Parse(a[5]));
            DEdge edgeVX = new DEdge(ref nodeV, ref nodeX, Int32.Parse(a[8]));
            DEdge edgeVW = new DEdge(ref nodeV, ref nodeW, Int32.Parse(a[10]));
            DEdge edgeXW = new DEdge(ref nodeX, ref nodeW, Int32.Parse(a[11]));
            DEdge edgeXY = new DEdge(ref nodeX, ref nodeY, Int32.Parse(a[12]));
            DEdge edgeYW = new DEdge(ref nodeY, ref nodeW, Int32.Parse(a[13]));
           
            network.AddEdge(ref edgeUV);
            network.AddEdge(ref edgeUX);
            network.AddEdge(ref edgeZW);
            network.AddEdge(ref edgeZY);
            network.AddEdge(ref edgeXY);
            network.AddEdge(ref edgeVW);
            network.AddEdge(ref edgeXW);
            network.AddEdge(ref edgeVX);
            network.AddEdge(ref edgeYW);
            network.AddEdge(ref edgeUY);
        }

        public static void BuildNetwork4(String[] a)
        {
            //define nodes with U as start node
            DNode nodeU = new DNode("u");
            DNode nodeV = new DNode("v");
            DNode nodeW = new DNode("w");
            DNode nodeX = new DNode("x");
            DNode nodeY = new DNode("y");
            DNode nodeZ = new DNode("z");
            nodeU.SetStartNode();

            //define network and add nodes to network
            network = new DNetwork(4);
            network.AddNode(ref nodeU);
            network.AddNode(ref nodeV);
            network.AddNode(ref nodeW);
            network.AddNode(ref nodeX);
            network.AddNode(ref nodeY);
            network.AddNode(ref nodeZ);

            //define edges and add to network
            DEdge edgeUV = new DEdge(ref nodeU, ref nodeV, Int32.Parse(a[0]));
            DEdge edgeUX = new DEdge(ref nodeU, ref nodeX, Int32.Parse(a[1]));
            DEdge edgeUW = new DEdge(ref nodeU, ref nodeW, Int32.Parse(a[2]));
            DEdge edgeUY = new DEdge(ref nodeU, ref nodeY, Int32.Parse(a[3]));
            DEdge edgeZW = new DEdge(ref nodeZ, ref nodeW, Int32.Parse(a[4]));
            DEdge edgeZY = new DEdge(ref nodeZ, ref nodeY, Int32.Parse(a[5]));
            DEdge edgeZV = new DEdge(ref nodeZ, ref nodeV, Int32.Parse(a[6]));
            DEdge edgeZX = new DEdge(ref nodeZ, ref nodeX, Int32.Parse(a[7]));
            DEdge edgeVX = new DEdge(ref nodeV, ref nodeX, Int32.Parse(a[8]));
            DEdge edgeVW = new DEdge(ref nodeV, ref nodeW, Int32.Parse(a[10]));
            DEdge edgeXY = new DEdge(ref nodeX, ref nodeY, Int32.Parse(a[12]));
            DEdge edgeYW = new DEdge(ref nodeY, ref nodeW, Int32.Parse(a[13]));

            network.AddEdge(ref edgeUV);
            network.AddEdge(ref edgeUX);
            network.AddEdge(ref edgeZW);
            network.AddEdge(ref edgeZY);
            network.AddEdge(ref edgeXY);
            network.AddEdge(ref edgeVW);
            network.AddEdge(ref edgeVX);
            network.AddEdge(ref edgeYW);
            network.AddEdge(ref edgeUW);
            network.AddEdge(ref edgeUY);
            network.AddEdge(ref edgeZV);
            network.AddEdge(ref edgeZX);
        }

        public static String[,] GetTableValues()
        {
            return tableValues;
        }

    }


    /**********************************************************************
    *********************************************************************/
    //Networks
    public class DNetwork
    {
        //VARIABLES
        public int networkID;
        public List<DNode> nodesList;
        public List<DEdge> edgesList;
        public List<DNode> visitedNodesList;
        public List<DNode> unvisitedNodesList;
        public List<DEdge> visitedEdgesList;
        public List<DEdge> unvisitedEdgesList;

        //CONSTRUCTOR
        public DNetwork(int id)
        {
            this.networkID = id;
            this.nodesList = new List<DNode>();
            this.edgesList = new List<DEdge>();
            this.visitedNodesList = new List<DNode>();
            this.unvisitedNodesList = new List<DNode>();
            this.visitedEdgesList = new List<DEdge>();
            this.unvisitedEdgesList = new List<DEdge>();
        }

        //METHODS
        public void AddNode(ref DNode node)
        {
            if (!this.nodesList.Contains(node))
            {
                this.nodesList.Add(node);
                this.unvisitedNodesList.Add(node);
            }
        }

        public void AddEdge(ref DEdge edge)
        {
            if (!this.edgesList.Contains(edge))
            {
                this.edgesList.Add(edge);
                this.unvisitedEdgesList.Add(edge);
            }
        }

        public void VisitNode(DNode node, int discovery)
        {
            if (this.nodesList.Contains(node)
                && !this.visitedNodesList.Contains(node)
                && this.unvisitedNodesList.Contains(node))
            {
                this.visitedNodesList.Add(node);
                this.unvisitedNodesList.Remove(node);
                node.SetDiscovery(discovery);
            }
        }

        public void VisitEdge(DNode a, DNode b)
        {
            DEdge edge = FindEdge(a, b);
            if (this.edgesList.Contains(edge)
            && !this.visitedEdgesList.Contains(edge)
            && this.unvisitedEdgesList.Contains(edge))
            {
                this.visitedEdgesList.Add(edge);
                this.unvisitedEdgesList.Remove(edge);
                edge.Visit();
            }
        }

        public DEdge FindEdge(DNode a, DNode b)
        {
            foreach(DEdge edge in this.edgesList)
            {
                DNode nodeA = edge.GetNodeA();
                DNode nodeB = edge.GetNodeB();

                if((nodeA == a && nodeB == b) || (nodeA == b && nodeB == a))
                {
                    return edge;
                }
            }
            return null;
        }

    }


    /**********************************************************************
    *********************************************************************/
    //Edges
    public class DEdge
    {
        //VARIABLES
        private DNode nodeA, nodeB;
        private int weight;
        private bool visited;

        //CONSTRUCTOR
        public DEdge(ref DNode a, ref DNode b, int w)
        {
            a.AddNeighbor(ref b);
            b.AddNeighbor(ref a);
            this.nodeA = a;
            this.nodeB = b;
            this.weight = w;
            this.visited = false;
        }

        //METHODS
        public DNode GetNodeA()
        {
            return this.nodeA;
        }

        public DNode GetNodeB()
        {
            return this.nodeB;
        }

        public int GetWeight()
        {
            return this.weight;
        }

        public void Visit()
        {
            this.visited = true;
        }

        public bool IsVisited()
        {
            return this.visited;
        }

        public bool Check(String a, String b)
        {
            if((this.nodeA.GetNodeName() == a && this.nodeB.GetNodeName() == b)
                || (this.nodeA.GetNodeName() == b && this.nodeB.GetNodeName() == a)){
                return true;
            }
            return false;
        }
    }


    /**********************************************************************
    *********************************************************************/
    //Nodes
    public class DNode
    {
        //VARIABLES
        private String name;
        private int weight;
        private int discovery;
        private bool isStartNode;
        private List<DNode> neighborsList;
        private DNode previousNode;

        //CONSTRUCTOR
        public DNode(String name)
        {
            this.name = name;
            this.isStartNode = false;
            this.discovery = -1;
            neighborsList = new List<DNode>();
            this.weight = 999999999;
        }

        //METHODS
        public void AddPreviousNode(ref DNode prev)
        {
            this.previousNode = prev;
        }

        public DNode GetPreviousNode()
        {
            return this.previousNode;
        }

        public String GetNodeName()
        {
            return this.name;
        }

        public int GetWeight()
        {
            return this.weight;
        }

        public void SetWeight(int weight)
        {
            this.weight = weight;
        }

        public int GetDiscovery()
        {
            return this.discovery;
        }

        public void SetDiscovery(int discovery)
        {
            this.discovery = discovery;
        }

        public bool IsVisited()
        {
            if(discovery == -1)
            {
                return false;
            }
            return true;
        }

        public bool GetIsStartNode()
        {
            return this.isStartNode;
        }

        public void SetStartNode()
        {
            this.isStartNode = true;
            this.previousNode = this;
            this.discovery = 0;
            this.weight = 0;
        }

        public void AddNeighbor(ref DNode neighbor)
        {
            if (!this.neighborsList.Contains(neighbor))
            {
                this.neighborsList.Add(neighbor);
            }
        }

        public ref List<DNode> GetNeighbors()
        {
            return ref this.neighborsList;
        }

        public bool IsNeighborOf(ref DNode other)
        {
            return this.neighborsList.Contains(other);
        }

    }
    /**********************************************************************
    *********************************************************************/
}
