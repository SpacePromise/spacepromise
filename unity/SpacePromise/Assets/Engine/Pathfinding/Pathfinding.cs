using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace Assets.Engine.Pathfinding
{
    public struct Int3
    {
        public readonly int X;
        public readonly int Y;
        public readonly int Z;

        private readonly int hashCode;

        public static readonly Int3 Zero = new Int3(0, 0, 0);


        public Int3(int x, int y, int z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

            unchecked
            {
                int localHashCode = x;
                localHashCode = (localHashCode * 397) ^ y;
                localHashCode = (localHashCode * 397) ^ z;
                this.hashCode = localHashCode;
            }
        }


        public static explicit operator Vector3(Int3 int3)
        {
            return new Vector3(int3.X, int3.Y, int3.Z);
        }

        public static explicit operator Int3(Vector3 vector3)
        {
            return new Int3((int)vector3.x, (int)vector3.y, (int)vector3.z);
        }

        public static double Distance(Int3 a, Int3 b)
        {
            var dx = b.X - a.X;
            var dy = b.Y - a.Y;
            var dz = b.Z - a.Z;
            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }

    public class GraphNode
    {
        public bool wasOpen;
        public bool IsBlocked;

        public readonly int Index;
        private readonly int hashCode;
        public readonly Int3 Location;
        public readonly List<GraphNode> ConnectionsList = new List<GraphNode>(8);
        public GraphNode[] Connections;
        

        public GraphNode(Int3 location)
        {
            this.Location = location;
            this.hashCode = location.GetHashCode();
            this.Index = (Location.Z + 100) * 200 + Location.X + 100;
        }


        public void Lock()
        {
            this.Connections = this.ConnectionsList.ToArray();
        }

        public override int GetHashCode()
        {
            return hashCode;
        }
    }

    public class PathNode
    {
        public readonly GraphNode GraphNode;

        public readonly uint FunctionCost;
        private readonly uint CostFromStart;

        public readonly uint DestinationHeuristic;
        public readonly PathNode From;
        public readonly bool IsEnd;

        private readonly int hashCode;

        public PathNode(GraphNode graphNode, GraphNode destinationNode)
            : this(graphNode, null, destinationNode)
        {
        }

        public PathNode(GraphNode graphNode, PathNode currentNode, GraphNode destinationNode)
        {
            this.From = currentNode;
            this.GraphNode = graphNode;

            if (this.From != null)
            {
                this.CostFromStart =
                    this.From.CostFromStart +
                    (uint) (Int3.Distance(this.From.GraphNode.Location, graphNode.Location) * 1000);
            }

            if (graphNode.Equals(destinationNode))
            {
                this.IsEnd = true;
                this.DestinationHeuristic = 0;
            }
            else
            {
                this.DestinationHeuristic = (uint)(Int3.Distance(destinationNode.Location, graphNode.Location) * 1000);
            }

            this.FunctionCost = this.CostFromStart + this.DestinationHeuristic;

            this.hashCode = this.GraphNode.GetHashCode();
        }

        public override int GetHashCode()
        {
            return this.hashCode;
        }
    }

    public class Graph
    {
        private readonly int width;
        private readonly int height;
        private readonly Int3 offset;
        public readonly GraphNode[,] Nodes;
        //public readonly List<GraphNode> Nodes = new List<GraphNode>();


        public Graph(int width, int height, Int3 offset)
        {
            this.width = width;
            this.height = height;
            this.offset = offset;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            this.Nodes = new GraphNode[this.height, this.width];
            for (int y = 0; y < this.height; y++)
            for (int x = 0; x < this.width; x++)
                this.Nodes[y, x] = new GraphNode(new Int3(this.offset.X + x, 0, this.offset.Z + y));
            
            sw.Stop();
            Debug.Log("Generated point mesh in " + sw.ElapsedTicks);
            sw.Reset();
            sw.Start();

            for (int y = 0; y < this.height; y++)
            {
                for (int x = 0; x < this.width; x++)
                {
                    var node = this.Nodes[y,x];
                    if (y > 1)
                    {
                        if (x > 0)
                        {
                            node.ConnectionsList.Add(this.Nodes[y - 1,x - 1]);
                        }
                        node.ConnectionsList.Add(this.Nodes[y - 1,x]);
                        if (x < this.width - 1)
                        {
                            node.ConnectionsList.Add(this.Nodes[y - 1,x + 1]);
                        }
                    }
                    if (x > 0)
                    {
                        node.ConnectionsList.Add(this.Nodes[y,x - 1]);
                    }
                    if (x < this.width - 1)
                    {
                        node.ConnectionsList.Add(this.Nodes[y,x + 1]);
                    }
                    if (y < this.height - 1)
                    {
                        if (x > 0)
                        {
                            node.ConnectionsList.Add(this.Nodes[y + 1,x - 1]);
                        }
                        node.ConnectionsList.Add(this.Nodes[y + 1,x]);
                        if (x < this.width - 1)
                        {
                            node.ConnectionsList.Add(this.Nodes[y + 1,x + 1]);
                        }
                    }
                    node.Lock();
                }
            }

            sw.Stop();
            Debug.Log("Generated mesh connections in " + sw.ElapsedTicks);
        }
    }

    public class Pathfinding : MonoBehaviour
    {
        private Graph graph;
        private PathNode lastFoundPath;
        private Vector3 lastDestinationLocation;
        private Vector3 size;
        private int graphSize;

        public void Start()
        {
            var collider = this.GetComponent<Collider>();
            this.size = collider.bounds.size;
            var localPosition = this.transform.position;

            this.graphSize = (int)size.x;
            this.graph = new Graph(
                (int)size.x,
                (int)size.z,
                new Int3((int)(-size.x / 2 + localPosition.x), 0, (int)(-size.z / 2 + localPosition.z)));
        }

        private void DetectObstacles()
        {
            if (graph == null) return;

            // Mark whole graph as not blocked
            var ySize = this.graph.Nodes.GetLength(0);
            var xSize = this.graph.Nodes.GetLength(1);
            for (int oY = 0; oY < size.z && oY < ySize; oY++)
            {
                for (int oX = 0; oX < size.x && oX < xSize; oX++)
                {
                    this.graph.Nodes[oY, oX].IsBlocked = false;
                }
            }

            // Add obstacles
            var obstacles = this.GetComponentsInChildren<PathfindingStaticObstacle>();
            foreach (var staticObstacle in obstacles)
            {
                var startX = (int)Mathf.Clamp((float)Math.Floor(staticObstacle.Bounds.x - staticObstacle.Bounds.width / 2 + size.x / 2), 0, size.x);
                var startY = (int)Mathf.Clamp((float)Math.Floor(staticObstacle.Bounds.y - staticObstacle.Bounds.height / 2 + size.z / 2), 0, size.z);
                var width = (int)Math.Ceiling(staticObstacle.Bounds.width + 3);
                var height = (int)Math.Ceiling(staticObstacle.Bounds.height + 3);

                for (int oY = startY; oY < size.z && oY < startY + height; oY++)
                {
                    for (int oX = startX; oX < size.x && oX < startX + width; oX++)
                    {
                        this.graph.Nodes[oY,oX].IsBlocked = true;
                    }
                }
            }
        }

        public int lineCount = 100;
        public float radius = 3.0f;

        static Material lineMaterial;
        static void CreateLineMaterial()
        {
            if (!lineMaterial)
            {
                // Unity has a built-in shader that is useful for drawing
                // simple colored things.
                Shader shader = Shader.Find("Hidden/Internal-Colored");
                lineMaterial = new Material(shader);
                lineMaterial.hideFlags = HideFlags.HideAndDontSave;
                // Turn on alpha blending
                lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                // Turn backface culling off
                lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
                // Turn off depth writes
                lineMaterial.SetInt("_ZWrite", 0);
            }
        }

        private int counter = 0;
        public void Update()
        {
            if (counter++ > 20)
            {
                this.DetectObstacles();
            }

            if (this.graph != null)
            {
                if (Input.mousePosition != this.lastDestinationLocation)
                {
                    var ray = UnityEngine.Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit rayInfo;
                    Physics.Raycast(ray, out rayInfo);
                    this.lastFoundPath = GetPath(this.graph.Nodes[0,0],
                        this.graph.Nodes
                            [(int) Mathf.Clamp(rayInfo.point.z + this.size.z / 2, 0, this.size.z),
                            (int) Mathf.Clamp(rayInfo.point.x + this.size.x / 2, 0, this.size.x)]);

                    lastDestinationLocation = Input.mousePosition;
                }
            }
        }

        public void OnRenderObject()
        {
            CreateLineMaterial();
            // Apply the line material
            lineMaterial.SetPass(0);

            GL.PushMatrix();
            // Set transformation matrix for drawing to
            // match our transform
            GL.MultMatrix(transform.localToWorldMatrix);

            // Draw lines
            GL.Begin(GL.LINES);
            if (lastFoundPath != null)
            {
                GL.Color(new Color(time / 10f, 0, 0, 1f));
                var currentNode = this.lastFoundPath;
                while (currentNode != null)
                {
                    if (currentNode.From != null)
                    {
                        GL.Vertex3(currentNode.GraphNode.Location.X, currentNode.GraphNode.Location.Y, currentNode.GraphNode.Location.Z);
                        GL.Vertex3(currentNode.From.GraphNode.Location.X, currentNode.From.GraphNode.Location.Y, currentNode.From.GraphNode.Location.Z);
                    }

                    currentNode = currentNode.From;
                }
            }
            GL.End();
            GL.PopMatrix();
        }

        private int opCounter = 0;
        private int time = 0;
        public PathNode GetPath(GraphNode startNode, GraphNode destinationNode)
        {
            // TODO Remove, for debugging
            var ySize = this.graph.Nodes.GetLength(0);
            var xSize = this.graph.Nodes.GetLength(1);
            for (int oY = 0; oY < size.z && oY < ySize; oY++)
                for (int oX = 0; oX < size.x && oX < xSize; oX++)
                    this.graph.Nodes[oY, oX].wasOpen = false;

            opCounter = 0;
            var sw1 = new Stopwatch();

            sw1.Start();
            var gcount = 0;
            for (int g = 0; g < 40000; g++)
            {
                gcount++;
            }
            sw1.Stop();

            var bestPath = new PathNode(startNode, destinationNode);
            var openNodes = new BinaryHeap(128);
            var visitedNodes = new bool[40000];
            openNodes.Add(bestPath);

            Stopwatch sw = new Stopwatch();
            sw.Start();
            do
            {
                //sw1.Start();
                var currentPath = openNodes.Remove();
                //sw1.Stop();
                visitedNodes[currentPath.GraphNode.Index] = true;

                if (currentPath.DestinationHeuristic < bestPath.DestinationHeuristic || currentPath.IsEnd)
                    bestPath = currentPath;

                if (currentPath.IsEnd)
                    break;

                // Go through connected nodes
                var neighbours = currentPath.GraphNode.Connections;
                var neighboursCount = neighbours.Length;
                for (int index = 0; index < neighboursCount; index++)
                {
                    var graphNode = neighbours[index];
                    var loc = graphNode.Index;
                    if (graphNode.IsBlocked ||
                        visitedNodes[loc])
                        continue;
                    visitedNodes[loc] = true;

                    // TODO Remove, for debugging
                    graphNode.wasOpen = true;
                    //sw1.Start();
                    var newNode = new PathNode(graphNode, currentPath, destinationNode);
                    openNodes.Add(newNode);
                    //sw1.Stop();
                }

                opCounter++;
            } while (openNodes.NumberOfItems > 0);

            sw.Stop();
            Debug.Log("Found path in " + sw.ElapsedTicks + " ticks");
            Debug.Log("Total neighbours handling: " + sw1.ElapsedTicks);
            Debug.Log("Total operations: " + opCounter);
            time = (int)sw.ElapsedMilliseconds;

            return bestPath;
        }

        //private List<GraphNode> GetInterestingNeighvours(GraphNode start, GraphNode end)
        //{
            
        //}

        public void OnDrawGizmos()
        {
            if (graph == null)
                return;

            var ySize = this.graph.Nodes.GetLength(0);
            var xSize = this.graph.Nodes.GetLength(1);
            for (int oY = 0; oY < size.z && oY < ySize; oY++)
            {
                for (int oX = 0; oX < size.x && oX < xSize; oX++)
                {
                    var node = this.graph.Nodes[oY, oX];
                    for (int cIndex = 0; cIndex < node.Connections.Length; cIndex++)
                    {
                        var conn = node.Connections[cIndex];
                        if (conn.wasOpen)
                            Gizmos.color = Color.blue;

                        if (!conn.IsBlocked)
                            Gizmos.DrawLine((Vector3)node.Location, (Vector3)conn.Location);
                        Gizmos.color = Color.white;
                    }
                }
            }

            if (lastFoundPath != null)
            {
                Gizmos.color = Color.red;
                var currentNode = this.lastFoundPath;
                while (currentNode != null)
                {
                    if (currentNode.From != null)
                        Gizmos.DrawLine((Vector3)currentNode.GraphNode.Location, (Vector3)currentNode.From.GraphNode.Location);

                    currentNode = currentNode.From;
                }
            }
        }
    }

    //class Pathfinding : MonoBehaviour
    //{
    //    void Awake()
    //    {
    //    }

    //    void OnDrawGizmos()
    //    {
    //        Gizmos.color = Color.white;
    //        Gizmos.DrawLine(new Vector3(0,1,0), new Vector3(10,1,10));
    //    }
    //}
}
