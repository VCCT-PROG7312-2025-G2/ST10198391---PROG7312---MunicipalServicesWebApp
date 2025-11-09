using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.DataStructures
{
    public enum NodeColor
    {
        Red,
        Black
    }

    public class RedBlackTreeNode
    {
        public ServiceRequest Data { get; set; }
        public RedBlackTreeNode? Left { get; set; }
        public RedBlackTreeNode? Right { get; set; }
        public RedBlackTreeNode? Parent { get; set; }
        public NodeColor Color { get; set; }

        public RedBlackTreeNode(ServiceRequest data)
        {
            Data = data;
            Color = NodeColor.Red;
        }
    }

    public class RedBlackTree
    {
        private RedBlackTreeNode? _root;

        private void RotateLeft(RedBlackTreeNode? node)
        {
            if (node?.Right == null) return;

            RedBlackTreeNode rightChild = node.Right;
            node.Right = rightChild.Left;

            if (rightChild.Left != null)
            {
                rightChild.Left.Parent = node;
            }

            rightChild.Parent = node.Parent;

            if (node.Parent == null)
            {
                _root = rightChild;
            }
            else if (node == node.Parent.Left)
            {
                node.Parent.Left = rightChild;
            }
            else
            {
                node.Parent.Right = rightChild;
            }

            rightChild.Left = node;
            node.Parent = rightChild;
        }

        private void RotateRight(RedBlackTreeNode? node)
        {
            if (node?.Left == null) return;

            RedBlackTreeNode leftChild = node.Left;
            node.Left = leftChild.Right;

            if (leftChild.Right != null)
            {
                leftChild.Right.Parent = node;
            }

            leftChild.Parent = node.Parent;

            if (node.Parent == null)
            {
                _root = leftChild;
            }
            else if (node == node.Parent.Right)
            {
                node.Parent.Right = leftChild;
            }
            else
            {
                node.Parent.Left = leftChild;
            }

            leftChild.Right = node;
            node.Parent = leftChild;
        }

        private void FixInsert(RedBlackTreeNode? node)
        {
            while (node != null && node.Parent != null && node.Parent.Color == NodeColor.Red)
            {
                if (node.Parent == node.Parent.Parent?.Left)
                {
                    RedBlackTreeNode? uncle = node.Parent.Parent.Right;

                    if (uncle != null && uncle.Color == NodeColor.Red)
                    {
                        node.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        node.Parent.Parent.Color = NodeColor.Red;
                        node = node.Parent.Parent;
                    }
                    else
                    {
                        if (node == node.Parent.Right)
                        {
                            node = node.Parent;
                            RotateLeft(node);
                        }

                        if (node?.Parent != null)
                        {
                            node.Parent.Color = NodeColor.Black;
                            if (node.Parent.Parent != null)
                            {
                                node.Parent.Parent.Color = NodeColor.Red;
                                RotateRight(node.Parent.Parent);
                            }
                        }
                    }
                }
                else
                {
                    RedBlackTreeNode? uncle = node.Parent.Parent?.Left;

                    if (uncle != null && uncle.Color == NodeColor.Red)
                    {
                        node.Parent.Color = NodeColor.Black;
                        uncle.Color = NodeColor.Black;
                        if (node.Parent.Parent != null)
                        {
                            node.Parent.Parent.Color = NodeColor.Red;
                            node = node.Parent.Parent;
                        }
                    }
                    else
                    {
                        if (node == node.Parent.Left)
                        {
                            node = node.Parent;
                            RotateRight(node);
                        }

                        if (node?.Parent != null)
                        {
                            node.Parent.Color = NodeColor.Black;
                            if (node.Parent.Parent != null)
                            {
                                node.Parent.Parent.Color = NodeColor.Red;
                                RotateLeft(node.Parent.Parent);
                            }
                        }
                    }
                }
            }

            if (_root != null)
            {
                _root.Color = NodeColor.Black;
            }
        }

        public void Insert(ServiceRequest request)
        {
            RedBlackTreeNode newNode = new RedBlackTreeNode(request);

            if (_root == null)
            {
                _root = newNode;
                _root.Color = NodeColor.Black;
                return;
            }

            RedBlackTreeNode? current = _root;
            RedBlackTreeNode? parent = null;

            while (current != null)
            {
                parent = current;
                int comparison = CompareRequests(current.Data, request);
                if (comparison > 0)
                {
                    current = current.Left;
                }
                else if (comparison < 0)
                {
                    current = current.Right;
                }
                else
                {
                    return; // Duplicate
                }
            }

            newNode.Parent = parent;
            if (parent != null)
            {
                int comparison = CompareRequests(parent.Data, request);
                if (comparison > 0)
                {
                    parent.Left = newNode;
                }
                else
                {
                    parent.Right = newNode;
                }
            }

            FixInsert(newNode);
        }

        public ServiceRequest? Search(Guid id)
        {
            return SearchRecursive(_root, id)?.Data;
        }

        private RedBlackTreeNode? SearchRecursive(RedBlackTreeNode? node, Guid id)
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

        private void InOrderTraversalRecursive(RedBlackTreeNode? node, List<ServiceRequest> result)
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

