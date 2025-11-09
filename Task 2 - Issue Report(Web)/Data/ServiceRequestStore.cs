using System.Linq;
using System.Text.Json;
using Task_2___Issue_Report_Web_.DataStructures;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.Data
{
    public class ServiceRequestStore
    {
        private readonly string _dataFilePath;
        private readonly List<ServiceRequest> _requests = new();
        private BinarySearchTree _bst = new();
        private AVLTree _avlTree = new();
        private RedBlackTree _redBlackTree = new();
        private MinHeap _priorityHeap = new();
        private ServiceRequestGraph _graph = new();
        private readonly Dictionary<string, ServiceRequest> _requestIdLookup = new(StringComparer.OrdinalIgnoreCase);
        private readonly Dictionary<Guid, ServiceRequest> _idLookup = new();
        private readonly object _lock = new();

        public ServiceRequestStore(string contentRootPath)
        {
            _dataFilePath = Path.Combine(contentRootPath, "App_Data", "serviceRequests.json");
            Load();
            InitializeDataStructures();
        }

        private void Load()
        {
            try
            {
                var dir = Path.GetDirectoryName(_dataFilePath);
                if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);

                if (!File.Exists(_dataFilePath))
                {
                    // Migrate existing issues to service requests
                    MigrateFromIssues();
                    Save();
                }
                else
                {
                    var json = File.ReadAllText(_dataFilePath);
                    var list = JsonSerializer.Deserialize<List<ServiceRequest>>(json) ?? new List<ServiceRequest>();
                    _requests.AddRange(list);
                }
            }
            catch
            {
                MigrateFromIssues();
            }
        }

        private void MigrateFromIssues()
        {
            // Migrate existing issues to service requests
            try
            {
                var issuesPath = Path.Combine(Path.GetDirectoryName(_dataFilePath)!, "issues.json");
                if (File.Exists(issuesPath))
                {
                    var json = File.ReadAllText(issuesPath);
                    var issues = JsonSerializer.Deserialize<List<Issue>>(json) ?? new List<Issue>();

                    foreach (var issue in issues)
                    {
                        var request = new ServiceRequest
                        {
                            Id = issue.Id,
                            RequestId = GenerateRequestId(),
                            SubmittedAt = issue.SubmittedAt,
                            Location = issue.Location,
                            Category = issue.Category,
                            Description = issue.Description,
                            Attachments = issue.Attachments,
                            Status = DetermineInitialStatus(issue),
                            Priority = DeterminePriority(issue),
                            UpdatedAt = issue.SubmittedAt
                        };
                        _requests.Add(request);
                    }
                }
            }
            catch
            {
                // If migration fails, start with empty list
            }
        }

        private string GenerateRequestId()
        {
            // Generate a unique request ID like REQ-2025-001234
            var year = DateTime.UtcNow.Year;
            var random = new Random();
            string requestId;
            int attempts = 0;

            // Ensure uniqueness
            do
            {
                var number = random.Next(100000, 999999);
                requestId = $"REQ-{year}-{number:D6}";
                attempts++;
            } while (_requestIdLookup.ContainsKey(requestId) && attempts < 100);

            // Fallback to GUID-based ID if we can't generate a unique numeric one
            if (_requestIdLookup.ContainsKey(requestId))
            {
                var guidPart = Guid.NewGuid().ToString("N").ToUpper();
                requestId = $"REQ-{year}-{guidPart.Substring(0, Math.Min(6, guidPart.Length))}";
            }

            return requestId;
        }

        private RequestStatus DetermineInitialStatus(Issue issue)
        {
            // Determine status based on age of issue
            var daysSinceSubmission = (DateTime.UtcNow - issue.SubmittedAt).TotalDays;
            if (daysSinceSubmission > 30)
            {
                return RequestStatus.Resolved;
            }
            else if (daysSinceSubmission > 14)
            {
                return RequestStatus.InProgress;
            }
            else if (daysSinceSubmission > 7)
            {
                return RequestStatus.UnderReview;
            }
            return RequestStatus.Submitted;
        }

        private int DeterminePriority(Issue issue)
        {
            // Determine priority based on category and description
            var category = issue.Category.ToLower();
            var description = issue.Description.ToLower();

            if (category.Contains("water") || category.Contains("utilities") || description.Contains("emergency") || description.Contains("urgent"))
            {
                return 1; // Highest priority
            }
            else if (category.Contains("roads") || category.Contains("potholes") || description.Contains("dangerous") || description.Contains("hazard"))
            {
                return 2;
            }
            else if (category.Contains("lighting") && description.Contains("dark") || description.Contains("unsafe"))
            {
                return 3;
            }
            else if (category.Contains("sanitation"))
            {
                return 4;
            }
            return 5; // Default priority
        }

        private void InitializeDataStructures()
        {
            foreach (var request in _requests)
            {
                _bst.Insert(request);
                _avlTree.Insert(request);
                _redBlackTree.Insert(request);
                _priorityHeap.Insert(request);
                _requestIdLookup[request.RequestId] = request;
                _idLookup[request.Id] = request;
            }

            // Build graph based on relationships
            _graph.BuildCategoryGraph(_requests);
        }

        private void Save()
        {
            lock (_lock)
            {
                var json = JsonSerializer.Serialize(_requests, new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(_dataFilePath, json);
            }
        }

        public void Add(ServiceRequest request)
        {
            lock (_lock)
            {
                if (string.IsNullOrEmpty(request.RequestId))
                {
                    request.RequestId = GenerateRequestId();
                }

                _requests.Add(request);
                _bst.Insert(request);
                _avlTree.Insert(request);
                _redBlackTree.Insert(request);
                _priorityHeap.Insert(request);
                _requestIdLookup[request.RequestId] = request;
                _idLookup[request.Id] = request;

                // Rebuild graph to include new request
                _graph.AddNode(request);
                foreach (var existingRequest in _requests)
                {
                    if (existingRequest.Id != request.Id)
                    {
                        var weight = CalculateRelationshipWeight(request, existingRequest);
                        if (weight > 0)
                        {
                            _graph.AddEdge(request.Id, existingRequest.Id, 1.0 / weight);
                        }
                    }
                }

                Save();
            }
        }

        private double CalculateRelationshipWeight(ServiceRequest a, ServiceRequest b)
        {
            double weight = 0;
            if (a.Category == b.Category) weight += 1.0;
            if (a.Location.Equals(b.Location, StringComparison.OrdinalIgnoreCase)) weight += 0.5;
            if (a.Status == b.Status) weight += 0.3;
            var timeDiff = Math.Abs((a.SubmittedAt - b.SubmittedAt).TotalHours);
            if (timeDiff < 24) weight += 0.2;
            return weight;
        }

        public void Update(ServiceRequest request)
        {
            lock (_lock)
            {
                var existing = _idLookup.GetValueOrDefault(request.Id);
                if (existing != null)
                {
                    existing.Status = request.Status;
                    existing.StatusNotes = request.StatusNotes;
                    existing.UpdatedAt = DateTime.UtcNow;
                    existing.Priority = request.Priority;
                    existing.AssignedTo = request.AssignedTo;
                    existing.EstimatedCompletionDate = request.EstimatedCompletionDate;

                    // Rebuild data structures with updated data
                    RebuildDataStructures();
                    Save();
                }
            }
        }

        private void RebuildDataStructures()
        {
            _bst = new BinarySearchTree();
            _avlTree = new AVLTree();
            _redBlackTree = new RedBlackTree();
            _priorityHeap = new MinHeap();
            _graph = new ServiceRequestGraph();
            _requestIdLookup.Clear();
            _idLookup.Clear();

            InitializeDataStructures();
        }

        public ServiceRequest? GetByRequestId(string requestId)
        {
            return _requestIdLookup.TryGetValue(requestId, out var request) ? request : null;
        }

        public ServiceRequest? GetById(Guid id)
        {
            return _idLookup.TryGetValue(id, out var request) ? request : null;
        }

        // Search using Binary Search Tree
        public ServiceRequest? SearchBST(Guid id)
        {
            return _bst.Search(id);
        }

        // Search using AVL Tree
        public ServiceRequest? SearchAVL(Guid id)
        {
            return _avlTree.Search(id);
        }

        // Search using Red-Black Tree
        public ServiceRequest? SearchRBT(Guid id)
        {
            return _redBlackTree.Search(id);
        }

        // Get all requests sorted by submission date (using BST in-order traversal)
        public List<ServiceRequest> GetAllSortedByDate()
        {
            return _bst.InOrderTraversal();
        }

        // Get requests by status using graph traversal
        public List<ServiceRequest> GetRequestsByStatus(RequestStatus status)
        {
            var result = new List<ServiceRequest>();
            var allNodes = _graph.GetAllNodes();

            foreach (var node in allNodes)
            {
                if (node.Request.Status == status)
                {
                    result.Add(node.Request);
                }
            }

            return result.OrderByDescending(r => r.SubmittedAt).ToList();
        }

        // Get highest priority requests using heap
        public List<ServiceRequest> GetTopPriorityRequests(int count)
        {
            var result = new List<ServiceRequest>();
            var tempHeap = new MinHeap();

            // Copy requests to temp heap
            foreach (var request in _requests)
            {
                tempHeap.Insert(request);
            }

            // Extract top priority requests
            for (int i = 0; i < count && !tempHeap.IsEmpty; i++)
            {
                var request = tempHeap.ExtractMin();
                if (request != null)
                {
                    result.Add(request);
                }
            }

            return result;
        }

        // Get related requests using graph
        public List<ServiceRequest> GetRelatedRequests(Guid requestId, int maxResults = 5)
        {
            var node = _graph.GetNode(requestId);
            if (node == null)
            {
                return new List<ServiceRequest>();
            }

            var related = new List<(ServiceRequest request, double weight)>();
            foreach (var edge in node.Edges)
            {
                related.Add((edge.To.Request, edge.Weight));
            }

            return related
                .OrderBy(r => r.weight)
                .Take(maxResults)
                .Select(r => r.request)
                .ToList();
        }

        // Get minimum spanning tree of requests
        public List<ServiceRequest> GetMinimumSpanningTreeRequests()
        {
            var mst = _graph.MinimumSpanningTree();
            var requests = new HashSet<ServiceRequest>();

            foreach (var edge in mst)
            {
                requests.Add(edge.From.Request);
                requests.Add(edge.To.Request);
            }

            return requests.ToList();
        }

        // Get all requests
        public List<ServiceRequest> GetAll()
        {
            return _requests.ToList();
        }

        // Get requests by category using graph BFS
        public List<ServiceRequest> GetRequestsByCategory(string category)
        {
            var result = new List<ServiceRequest>();
            var categoryRequests = _requests.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();

            if (categoryRequests.Count == 0)
            {
                return result;
            }

            // Use BFS from first request in category to find related requests
            var startId = categoryRequests[0].Id;
            var bfsResult = _graph.BreadthFirstSearch(startId);

            // Filter by category
            return bfsResult.Where(r => r.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToList();
        }
    }
}

