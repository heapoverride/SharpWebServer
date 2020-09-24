using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpWebServer
{
    public class Queue
    {
        private List<object> queue = new List<object>();

        public Queue()
        {

        }

        public long Add<T>(T item)
        {
            queue.Add(item);
            return queue.Count;
        }

        public bool IsEmpty()
        {
            return queue.Count == 0;
        }

        public T Fetch<T>()
        {
            if (queue.Count > 0)
            {
                lock (queue)
                {
                    T item = (T)queue[0];
                    queue.RemoveAt(0);

                    return item;
                }
            }

            return default(T);
        }

        public T[] Fetch<T>(int amount)
        {
            List<T> items = new List<T>();

            while (!IsEmpty() && items.Count < amount)
            {
                items.Add(Fetch<T>());
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