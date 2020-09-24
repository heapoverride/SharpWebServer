using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpWebServer
{
    public class Queue
    {
        private long size = -1;
        private List<object> queue = new List<object>();

        public Queue()
        {
        }

        public Queue(long size)
        {
            this.size = size;
        }

        public bool IsFull()
        {
            return size != -1 && queue.Count == size;
        }

        public void Add<T>(T item)
        {
            if (IsFull())
            {
                queue.RemoveAt(0);
            }
            queue.Add(item);
        }

        public bool IsEmpty()
        {
            return queue.Count == 0;
        }

        public T Fetch<T>()
        {
            if (queue.Count > 0)
            {
                T item = (T)queue[0];
                queue.RemoveAt(0);

                return item;
            }

            return default(T);
        }

        public T[] Fetch<T>(int amount)
        {
            List<T> items = new List<T>();

            lock (queue)
            {
                while (!IsEmpty() && items.Count < amount)
                {
                    items.Add(Fetch<T>());
                }
            }

            return items.ToArray();
        }

        public T[] FetchAll<T>()
        {
            T[] array = new T[queue.Count];

            lock (queue)
            {
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = (T)queue[i];
                }
                queue.Clear();
            }

            return array.ToArray<T>();
        }
    }

}