using System.Collections;
using System.Collections.Generic;
using Task_2___Issue_Report_Web_.Models;

namespace Task_2___Issue_Report_Web_.DataStructures
{
    public class MinHeap : IEnumerable<ServiceRequest>
    {
        private readonly List<ServiceRequest> _heap = new();

        private int Parent(int index) => (index - 1) / 2;
        private int LeftChild(int index) => 2 * index + 1;
        private int RightChild(int index) => 2 * index + 2;

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = Parent(index);
                if (Compare(_heap[index], _heap[parent]) >= 0)
                {
                    break;
                }
                Swap(index, parent);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = LeftChild(index);
                int right = RightChild(index);
                int smallest = index;

                if (left < _heap.Count && Compare(_heap[left], _heap[smallest]) < 0)
                {
                    smallest = left;
                }

                if (right < _heap.Count && Compare(_heap[right], _heap[smallest]) < 0)
                {
                    smallest = right;
                }

                if (smallest == index)
                {
                    break;
                }

                Swap(index, smallest);
                index = smallest;
            }
        }

        public void Insert(ServiceRequest request)
        {
            _heap.Add(request);
            HeapifyUp(_heap.Count - 1);
        }

        public ServiceRequest? ExtractMin()
        {
            if (_heap.Count == 0)
            {
                return null;
            }

            ServiceRequest min = _heap[0];
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return min;
        }

        public ServiceRequest? Peek()
        {
            return _heap.Count > 0 ? _heap[0] : null;
        }

        public int Count => _heap.Count;

        public bool IsEmpty => _heap.Count == 0;

        private int Compare(ServiceRequest a, ServiceRequest b)
        {
            // Compare by priority (lower number = higher priority), then by submitted date
            int priorityComparison = a.Priority.CompareTo(b.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }
            return a.SubmittedAt.CompareTo(b.SubmittedAt);
        }

        public IEnumerator<ServiceRequest> GetEnumerator()
        {
            return _heap.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

    public class MaxHeap
    {
        private readonly List<ServiceRequest> _heap = new();

        private int Parent(int index) => (index - 1) / 2;
        private int LeftChild(int index) => 2 * index + 1;
        private int RightChild(int index) => 2 * index + 2;

        private void Swap(int i, int j)
        {
            (_heap[i], _heap[j]) = (_heap[j], _heap[i]);
        }

        private void HeapifyUp(int index)
        {
            while (index > 0)
            {
                int parent = Parent(index);
                if (Compare(_heap[index], _heap[parent]) <= 0)
                {
                    break;
                }
                Swap(index, parent);
                index = parent;
            }
        }

        private void HeapifyDown(int index)
        {
            while (true)
            {
                int left = LeftChild(index);
                int right = RightChild(index);
                int largest = index;

                if (left < _heap.Count && Compare(_heap[left], _heap[largest]) > 0)
                {
                    largest = left;
                }

                if (right < _heap.Count && Compare(_heap[right], _heap[largest]) > 0)
                {
                    largest = right;
                }

                if (largest == index)
                {
                    break;
                }

                Swap(index, largest);
                index = largest;
            }
        }

        public void Insert(ServiceRequest request)
        {
            _heap.Add(request);
            HeapifyUp(_heap.Count - 1);
        }

        public ServiceRequest? ExtractMax()
        {
            if (_heap.Count == 0)
            {
                return null;
            }

            ServiceRequest max = _heap[0];
            _heap[0] = _heap[_heap.Count - 1];
            _heap.RemoveAt(_heap.Count - 1);

            if (_heap.Count > 0)
            {
                HeapifyDown(0);
            }

            return max;
        }

        public ServiceRequest? Peek()
        {
            return _heap.Count > 0 ? _heap[0] : null;
        }

        public int Count => _heap.Count;

        public bool IsEmpty => _heap.Count == 0;

        private int Compare(ServiceRequest a, ServiceRequest b)
        {
            // Compare by priority (higher number = higher priority), then by submitted date
            int priorityComparison = a.Priority.CompareTo(b.Priority);
            if (priorityComparison != 0)
            {
                return priorityComparison;
            }
            return b.SubmittedAt.CompareTo(a.SubmittedAt);
        }
    }
}

