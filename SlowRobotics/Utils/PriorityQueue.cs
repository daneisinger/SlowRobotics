using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SlowRobotics.Utils
{
    public class PriorityQueue<T> where T : IComparable<T>
    {
        private List<T> data;

        public PriorityQueue()
        {
            this.data = new List<T>();
        }
        public void Enqueue(T item)
        {
            data.Add(item);
            int ci = data.Count - 1;
            while (ci > 0)
            {
                int pi = (ci - 1) / 2;
                if (data[ci].CompareTo(data[pi]) >= 0)
                    break;
                T tmp = data[ci]; data[ci] = data[pi]; data[pi] = tmp;
                ci = pi;
            }
        }
        public T Dequeue()
        {
            // Assumes pq isn't empty
            int li = data.Count - 1;
            T frontItem = data[0];
            data[0] = data[li];
            data.RemoveAt(li);

            --li;
            int pi = 0;
            while (true)
            {
                int ci = pi * 2 + 1;
                if (ci > li) break;
                int rc = ci + 1;
                if (rc <= li && data[rc].CompareTo(data[ci]) < 0)
                    ci = rc;
                if (data[pi].CompareTo(data[ci]) <= 0) break;
                T tmp = data[pi]; data[pi] = data[ci]; data[ci] = tmp;
                pi = ci;
            }
            return frontItem;
        }
        public int Count()
        {
            return data.Count;
        }
        public T Peek()
        {
            T frontItem = data[0];
            return frontItem;
        }
        public T Take()
        {
            T frontItem = data[0];
            data.RemoveAt(0);
            return frontItem;
        }

        public List<T> getData()
        {
            return data;
        }
    }
}
