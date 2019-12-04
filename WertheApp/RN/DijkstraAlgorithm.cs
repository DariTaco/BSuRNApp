using System;
using System.Collections.Generic;
using System.Diagnostics; //Debug.WriteLine("");
using System.Linq;

namespace WertheApp.RN
{

    public class DijkstraAlgorithm
    {
        //VARIABLES
        private static String[,] tableValues;
        private static DNetwork network;

        //CONSTRUCTOR
        public DijkstraAlgorithm(String[] a)
        {
            //only needed when creating an object
        }

        //METHODS
        public static void Initialize()
        {
            //visit very first node (start node)
            network.VisitNode(network.nodesList.First());

            //find neighbors
            foreach (DNode neighbor in network.visitedNodesList.Last().GetNeighbors())
            {
                //assign weight of connecting edge to neighbor
                int weight = GetWeightFromEdge(network.visitedNodesList.Last(), neighbor);
                neighbor.SetWeight(weight);
            }

            FindMinimalWeight(network.visitedNodesList.Last());
        }

        public static void FindMinimalWeight(DNode node)
        {
            //network.visitedNodesList.Last()
            //find neighbor of start node with the minimal edge cost
            int minimalWeight = 999999999;
            DNode minimalNeighbor = null;
            foreach (DNode neighbor in node.GetNeighbors())
            {
              
                Debug.WriteLine("neighbors " + neighbor.GetNodeName() + " weight " + neighbor.GetWeight());
                //find neighbor with minimal weight
                if (neighbor.GetWeight() <= minimalWeight)
                {
                    minimalWeight = neighbor.GetWeight();
                    minimalNeighbor = neighbor;
                }
            }
            Debug.WriteLine("final minimal weight " + minimalWeight);

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

        public static void BuildNetwork1(String[] a)
        {
            //define nodes with U as start node
            DNode nodeU = new DNode("U");
            DNode nodeV = new DNode("V");
            DNode nodeW = new DNode("W");
            DNode nodeX = new DNode("X");
            DNode nodeY = new DNode("Y");
            DNode nodeZ = new DNode("Z");
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
            DNode nodeU = new DNode("U");
            DNode nodeV = new DNode("V");
            DNode nodeW = new DNode("W");
            DNode nodeX = new DNode("X");
            DNode nodeY = new DNode("Y");
            DNode nodeZ = new DNode("Z");

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
            DNode nodeU = new DNode("U");
            DNode nodeV = new DNode("V");
            DNode nodeW = new DNode("W");
            DNode nodeX = new DNode("X");
            DNode nodeY = new DNode("Y");
            DNode nodeZ = new DNode("Z");

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
            DNode nodeU = new DNode("U");
            DNode nodeV = new DNode("V");
            DNode nodeW = new DNode("W");
            DNode nodeX = new DNode("X");
            DNode nodeY = new DNode("Y");
            DNode nodeZ = new DNode("Z");

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

            PrintArray(a);
        }

        public static String[,] GetTableValues()
        {
            return tableValues;
        }

        public static void CreateTableValuesArray()
        {
            //∞
            //step, round(Dijkstra step), N', D(v), D(w), D(x), D(y), D(z)
            tableValues = new String[,] {
            { "0", "0", "u", "",        "",     "",     "",     "" },
            { "1", "0", "", "here",    "",     "",     "",     "" },
            { "2", "0", "", "",        "here", "",     "",     "" },
            { "3", "0", "", "",        "",     "here", "",     "" },
            { "4", "0", "", "",        "",     "",     "here", "" },
            { "5", "0", "", "",        "",     "",     "",     "here" },

            { "6",  "1", "u", "",        "",     "",     "",     "" },
            { "7",  "1", "", "x",        "",     "",     "",     "" },
            { "8",  "1", "", "",        "x",     "",     "",     "" },
            { "9",  "1", "", "",        "",     "x",     "",     "" },
            { "10", "1", "", "",        "",     "",     "x",     "" },
            { "11", "1", "", "",        "",     "",     "",     "x" },

            { "12", "2", "u", "",        "",     "",     "",     "" },
            { "13", "2", "", "x",        "",     "",     "",     "" },
            { "14", "2", "", "",        "x",     "",     "",     "" },
            { "15", "2", "", "",        "",     "x",     "",     "" },
            { "16", "2", "", "",        "",     "",     "x",     "" },
            { "17", "2", "", "",        "",     "",     "",     "x" }

            };
        }

        public static void PrintArray(String[] a)
        { 
            String[] b = {"weightUV ", "weightUX ", "weightZW ", "weightZY ",
                "weightXY ", "weightVW ", "weightVY ", "weightXW ", "weightVX ",
                "weightYW ", "weightUW ", "weightUY ", "weightZV ", "weightZX " };

            for(int i = 0; i < a.Length; i++)
            {
                Debug.WriteLine(b[i] + a[i]);
            }
        }
    }

    //Networks
    public class DNetwork
    {
        //VARIABLES
        public int networkID;
        public List<DNode> nodesList;
        public List<DEdge> edgesList;
        public List<DNode> visitedNodesList;
        public List<DNode> unvisitedNodesList;

        //CONSTRUCTOR
        public DNetwork(int id)
        {
            this.networkID = id;
            this.nodesList = new List<DNode>();
            this.edgesList = new List<DEdge>();
            this.visitedNodesList = new List<DNode>();
            this.unvisitedNodesList = new List<DNode>();
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
            }
        }

        public void VisitNode(DNode node)
        {
            if (this.nodesList.Contains(node)
                && !this.visitedNodesList.Contains(node)
                && this.unvisitedNodesList.Contains(node))
            {
                this.visitedNodesList.Add(node);
                this.unvisitedNodesList.Remove(node);
                Debug.WriteLine("Node " + node.GetNodeName() + " visited");
            }
        }

    }

    //Edges
    public class DEdge
    {
        //VARIABLES
        private DNode nodeA, nodeB;
        private int weight;

        //CONSTRUCTOR
        public DEdge(ref DNode a, ref DNode b, int w)
        {
            a.AddNeighbor(ref b);
            b.AddNeighbor(ref a);
            this.nodeA = a;
            this.nodeB = b;
            this.weight = w;
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
    }

    //Nodes
    public class DNode
    {
        //VARIABLES
        private String name;
        private int weight;
        private int discovery;
        private bool isStartNode;
        private List<DNode> neighborsList;

        //CONSTRUCTOR
        public DNode(String name)
        {
            this.name = name;
            this.isStartNode = false;
            this.discovery = -1;
            neighborsList = new List<DNode>();
        }

        //METHODS
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

        public bool GetIsStartNode()
        {
            return this.isStartNode;
        }

        public void SetStartNode()
        {
            this.isStartNode = true;
            this.discovery = 0;
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
}
