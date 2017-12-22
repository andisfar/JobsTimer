using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleTimerLib
{
    public class SingleTimersCollection : IDictionary<int, SingleTimer>
    {
        private Dictionary<int, SingleTimer> timers = new Dictionary<int, SingleTimer>();

        public Dictionary<int, SingleTimer> Timers
        {
            get { return timers; }
        }

        public SingleTimer this[int key]
        {
            get
            {
                return timers[key];
            }

            set
            {
                timers[key] = value;
            }
        }       

        public int Count
        {
            get
            {
                return timers.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public ICollection<int> Keys
        {
            get
            {
                return timers.Keys;
            }
        }

        public ICollection<SingleTimer> Values
        {
            get
            {
                return timers.Values;
            }
        }

        private bool _preserveTimers = false;
        public bool PreserveTimers { get=>_preserveTimers; set => _preserveTimers = value; }

        public void Add(KeyValuePair<int, SingleTimer> item)
        {
            Add(item.Key, item.Value);
        }

        public SingleTimerLib.SingleTimer AddTimer(int key, string canonicalNmae, string elapsedTimeOffset)
        {
            Add(key, new SingleTimer(key, canonicalNmae, elapsedTimeOffset));
            return this[key];
        }

        public void Add(int key, SingleTimer value)
        {
            timers.Add(key, value);
        }

        public void Clear()
        {
            timers.Clear();
        }

        public bool Contains(KeyValuePair<int, SingleTimer> item)
        {
            return timers.Contains(item);
        }

        public bool ContainsKey(int key)
        {
            return timers.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<int, SingleTimer>[] array, int arrayIndex)
        {
            KeyValuePair<int, SingleTimer> item = (KeyValuePair<int, SingleTimer>)timers.ToArray()[arrayIndex];
            array[arrayIndex] = item;
        }

        public IEnumerator<KeyValuePair<int, SingleTimer>> GetEnumerator()
        {
            return timers.GetEnumerator();
        }

        public bool Remove(KeyValuePair<int, SingleTimer> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(int key)
        {
            timers[key].Dispose();
            return timers.Remove(key);
        }

        public bool TryGetValue(int key, out SingleTimer value)
        {
            return timers.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return timers.GetEnumerator();
        }
    }
}
