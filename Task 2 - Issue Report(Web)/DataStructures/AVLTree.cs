using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.DataStructures
{
    public class AVLTreeNode
    {
        public ServiceRequest Data { get; set; }
        public AVLTreeNode? Left { get; set; }
        public AVLTreeNode? Right { get; set; }
        public int Height { get; set; }

        public AVLTreeNode(ServiceRequest data)
        {
            Data = data;
            Height = 1;
        }
    }

    public class AVLTree
    {
        private AVLTreeNode? _root;

        private int GetHeight(AVLTreeNode? node)
        {
            return node?.Height ?? 0;
        }

        private int GetBalanceFactor(AVLTreeNode? node)
        {
            if (node == null) return 0;
            return GetHeight(node.Left) - GetHeight(node.Right);
        }

        private AVLTreeNode RightRotate(AVLTreeNode y)
        {
            AVLTreeNode x = y.Left!;
            AVLTreeNode? T2 = x.Right;

            x.Right = y;
            y.Left = T2;

            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;
            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;

            return x;
        }

        private AVLTreeNode LeftRotate(AVLTreeNode x)
        {
            AVLTreeNode y = x.Right!;
            AVLTreeNode? T2 = y.Left;

            y.Left = x;
            x.Right = T2;

            x.Height = Math.Max(GetHeight(x.Left), GetHeight(x.Right)) + 1;
            y.Height = Math.Max(GetHeight(y.Left), GetHeight(y.Right)) + 1;

            return y;
        }

        public void Insert(ServiceRequest request)
        {
            _root = InsertRecursive(_root, request);
        }

        private AVLTreeNode InsertRecursive(AVLTreeNode? node, ServiceRequest request)
        {
            if (node == null)
            {
                return new AVLTreeNode(request);
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
            else
            {
                return node; 
            }

            node.Height = Math.Max(GetHeight(node.Left), GetHeight(node.Right)) + 1;

            int balance = GetBalanceFactor(node);

            // Left Left Case
            if (balance > 1 && CompareRequests(request, node.Left!.Data) < 0)
            {
                return RightRotate(node);
            }

            // Right Right Case
            if (balance < -1 && CompareRequests(request, node.Right!.Data) > 0)
            {
                return LeftRotate(node);
            }

            // Left Right Case
            if (balance > 1 && CompareRequests(request, node.Left!.Data) > 0)
            {
                node.Left = LeftRotate(node.Left);
                return RightRotate(node);
            }

            // Right Left Case
            if (balance < -1 && CompareRequests(request, node.Right!.Data) < 0)
            {
                node.Right = RightRotate(node.Right);
                return LeftRotate(node);
            }

            return node;
        }

        public ServiceRequest? Search(Guid id)
        {
            return SearchRecursive(_root, id)?.Data;
        }

        private AVLTreeNode? SearchRecursive(AVLTreeNode? node, Guid id)
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

        public List<ServiceRequest> InOrderTraversal()
        {
            var result = new List<ServiceRequest>();
            InOrderTraversalRecursive(_root, result);
            return result;
        }

        private void InOrderTraversalRecursive(AVLTreeNode? node, List<ServiceRequest> result)
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
            int dateComparison = a.SubmittedAt.CompareTo(b.SubmittedAt);
            if (dateComparison != 0)
            {
                return dateComparison;
            }
            return a.Id.CompareTo(b.Id);
        }
    }
}

