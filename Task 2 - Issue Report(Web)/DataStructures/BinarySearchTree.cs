using System.Collections;
using System.Collections.Generic;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.DataStructures
{
    public class BinarySearchTreeNode
    {
        public ServiceRequest Data { get; set; }
        public BinarySearchTreeNode? Left { get; set; }
        public BinarySearchTreeNode? Right { get; set; }

        public BinarySearchTreeNode(ServiceRequest data)
        {
            Data = data;
        }
    }

    public class BinarySearchTree : IEnumerable<ServiceRequest>
    {
        private BinarySearchTreeNode? _root;

        public void Insert(ServiceRequest request)
        {
            _root = InsertRecursive(_root, request);
        }

        private BinarySearchTreeNode InsertRecursive(BinarySearchTreeNode? node, ServiceRequest request)
        {
            if (node == null)
            {
                return new BinarySearchTreeNode(request);
            }

            int comparison = CompareRequests(node.Data, request);
            if (comparison > 0)
            {
                node.Left = InsertRecursive(node.Left, request);
            }
            else if (comparison < 0)
            {
                node.Right = InsertRecursive(node.Right, request);
            }

            return node;
        }

        public ServiceRequest? Search(Guid id)
        {
            return SearchRecursive(_root, id)?.Data;
        }

        private BinarySearchTreeNode? SearchRecursive(BinarySearchTreeNode? node, Guid id)
        {
            if (node == null || node.Data.Id == id)
            {
                return node;
            }

            if (node.Data.Id.CompareTo(id) > 0)
            {
                return SearchRecursive(node.Left, id);
            }

            return SearchRecursive(node.Right, id);
        }

        public ServiceRequest? SearchByRequestId(string requestId)
        {
            return SearchByRequestIdRecursive(_root, requestId)?.Data;
        }

        private BinarySearchTreeNode? SearchByRequestIdRecursive(BinarySearchTreeNode? node, string requestId)
        {
            if (node == null)
            {
                return null;
            }

            int comparison = string.Compare(node.Data.RequestId, requestId, StringComparison.OrdinalIgnoreCase);
            if (comparison == 0)
            {
                return node;
            }

            if (comparison > 0)
            {
                return SearchByRequestIdRecursive(node.Left, requestId);
            }

            return SearchByRequestIdRecursive(node.Right, requestId);
        }

        public List<ServiceRequest> InOrderTraversal()
        {
            var result = new List<ServiceRequest>();
            InOrderTraversalRecursive(_root, result);
            return result;
        }

        private void InOrderTraversalRecursive(BinarySearchTreeNode? node, List<ServiceRequest> result)
        {
            if (node != null)
            {
                InOrderTraversalRecursive(node.Left, result);
                result.Add(node.Data);
                InOrderTraversalRecursive(node.Right, result);
            }
        }

        private int CompareRequests(ServiceRequest a, ServiceRequest b)
        {
            // Compare by SubmittedAt, then by Id
            int dateComparison = a.SubmittedAt.CompareTo(b.SubmittedAt);
            if (dateComparison != 0)
            {
                return dateComparison;
            }
            return a.Id.CompareTo(b.Id);
        }

        public IEnumerator<ServiceRequest> GetEnumerator()
        {
            return InOrderTraversal().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}

