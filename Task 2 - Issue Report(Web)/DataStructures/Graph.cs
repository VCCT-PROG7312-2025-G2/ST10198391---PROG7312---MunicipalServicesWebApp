using System.Collections.Generic;
using System.Linq;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.DataStructures
{
    public class GraphNode
    {
        public ServiceRequest Request { get; set; }
        public List<GraphEdge> Edges { get; set; } = new();
        public bool Visited { get; set; }

        public GraphNode(ServiceRequest request)
        {
            Request = request;
        }
    }

    public class GraphEdge
    {
        public GraphNode From { get; set; }
        public GraphNode To { get; set; }
        public double Weight { get; set; }

        public GraphEdge(GraphNode from, GraphNode to, double weight)
        {
            From = from;
            To = to;
            Weight = weight;
        }
    }

    public class ServiceRequestGraph
    {
        private readonly Dictionary<Guid, GraphNode> _nodes = new();
        private readonly List<GraphEdge> _edges = new();

        public void AddNode(ServiceRequest request)
        {
            if (!_nodes.ContainsKey(request.Id))
            {
                _nodes[request.Id] = new GraphNode(request);
            }
        }

        public void AddEdge(Guid fromId, Guid toId, double weight)
        {
            if (_nodes.TryGetValue(fromId, out var fromNode) && _nodes.TryGetValue(toId, out var toNode))
            {
                var edge = new GraphEdge(fromNode, toNode, weight);
                fromNode.Edges.Add(edge);
                _edges.Add(edge);
            }
        }

        public GraphNode? GetNode(Guid id)
        {
            return _nodes.TryGetValue(id, out var node) ? node : null;
        }

        public List<GraphNode> GetAllNodes()
        {
            return _nodes.Values.ToList();
        }

        public List<GraphEdge> GetAllEdges()
        {
            return _edges.ToList();
        }

        // Depth-First Search
        public List<ServiceRequest> DepthFirstSearch(Guid startId)
        {
            var result = new List<ServiceRequest>();
            foreach (var node in _nodes.Values)
            {
                node.Visited = false;
            }

            if (_nodes.TryGetValue(startId, out var startNode))
            {
                DFSRecursive(startNode, result);
            }

            return result;
        }

        private void DFSRecursive(GraphNode node, List<ServiceRequest> result)
        {
            node.Visited = true;
            result.Add(node.Request);

            foreach (var edge in node.Edges)
            {
                if (!edge.To.Visited)
                {
                    DFSRecursive(edge.To, result);
                }
            }
        }

        // Breadth-First Search
        public List<ServiceRequest> BreadthFirstSearch(Guid startId)
        {
            var result = new List<ServiceRequest>();
            foreach (var node in _nodes.Values)
            {
                node.Visited = false;
            }

            if (!_nodes.TryGetValue(startId, out var startNode))
            {
                return result;
            }

            var queue = new Queue<GraphNode>();
            queue.Enqueue(startNode);
            startNode.Visited = true;

            while (queue.Count > 0)
            {
                var current = queue.Dequeue();
                result.Add(current.Request);

                foreach (var edge in current.Edges)
                {
                    if (!edge.To.Visited)
                    {
                        edge.To.Visited = true;
                        queue.Enqueue(edge.To);
                    }
                }
            }

            return result;
        }

        // Minimum Spanning Tree using Kruskal's Algorithm
        public List<GraphEdge> MinimumSpanningTree()
        {
            var mst = new List<GraphEdge>();
            var sortedEdges = _edges.OrderBy(e => e.Weight).ToList();
            var parent = new Dictionary<Guid, Guid>();
            var rank = new Dictionary<Guid, int>();

            // Initialize parent and rank
            foreach (var node in _nodes.Values)
            {
                parent[node.Request.Id] = node.Request.Id;
                rank[node.Request.Id] = 0;
            }

            Guid Find(Guid id)
            {
                if (parent[id] != id)
                {
                    parent[id] = Find(parent[id]); // Path compression
                }
                return parent[id];
            }

            void Union(Guid x, Guid y)
            {
                var rootX = Find(x);
                var rootY = Find(y);

                if (rootX == rootY) return;

                // Union by rank
                if (rank[rootX] < rank[rootY])
                {
                    parent[rootX] = rootY;
                }
                else if (rank[rootX] > rank[rootY])
                {
                    parent[rootY] = rootX;
                }
                else
                {
                    parent[rootY] = rootX;
                    rank[rootX]++;
                }
            }

            foreach (var edge in sortedEdges)
            {
                var fromRoot = Find(edge.From.Request.Id);
                var toRoot = Find(edge.To.Request.Id);

                if (fromRoot != toRoot)
                {
                    mst.Add(edge);
                    Union(edge.From.Request.Id, edge.To.Request.Id);

                    if (mst.Count >= _nodes.Count - 1)
                    {
                        break;
                    }
                }
            }

            return mst;
        }

        // Build graph based on category relationships
        public void BuildCategoryGraph(List<ServiceRequest> requests)
        {
            foreach (var request in requests)
            {
                AddNode(request);
            }

            // Create edges based on category similarity and location proximity
            var requestsList = requests.ToList();
            for (int i = 0; i < requestsList.Count; i++)
            {
                for (int j = i + 1; j < requestsList.Count; j++)
                {
                    var req1 = requestsList[i];
                    var req2 = requestsList[j];

                    double weight = CalculateWeight(req1, req2);
                    if (weight > 0)
                    {
                        AddEdge(req1.Id, req2.Id, weight);
                    }
                }
            }
        }

        private double CalculateWeight(ServiceRequest a, ServiceRequest b)
        {
            double weight = 0;

            // Category match adds weight
            if (a.Category == b.Category)
            {
                weight += 1.0;
            }

            // Location similarity (simplified - same location name)
            if (a.Location.Equals(b.Location, StringComparison.OrdinalIgnoreCase))
            {
                weight += 0.5;
            }

            // Status similarity
            if (a.Status == b.Status)
            {
                weight += 0.3;
            }

            // Time proximity (requests submitted close together)
            var timeDiff = Math.Abs((a.SubmittedAt - b.SubmittedAt).TotalHours);
            if (timeDiff < 24)
            {
                weight += 0.2;
            }

            return weight > 0 ? 1.0 / weight : 0; // Inverse weight for MST (lower weight = stronger connection)
        }
    }
}

