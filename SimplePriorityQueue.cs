using System;
using System.Collections.Generic;
using System.Linq;

namespace FilesCompressionProject
{
    public class SimplePriorityQueue<T>
    {
        private List<(T Item, int Priority)> _items = new List<(T, int)>();

        public int Count => _items.Count;

        public void Enqueue(T item, int priority)
        {
            _items.Add((item, priority));
            _items = _items.OrderBy(x => x.Priority).ToList();
        }

        public T Dequeue()
        {
            if (_items.Count == 0)
                throw new InvalidOperationException("The priority queue is empty.");

            var item = _items[0];
            _items.RemoveAt(0);
            return item.Item;
        }
    }
}
