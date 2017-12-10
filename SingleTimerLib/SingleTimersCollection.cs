using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SingleTimerLib
{
    public class SingleTimersCollection : IDictionary<string, SingleTimer>
    {
        private Dictionary<string, SingleTimer> timers = new Dictionary<string, SingleTimer>();

        public Dictionary<string, SingleTimer> Timers
        {
            get { return timers; }
        }

        public SingleTimer this[string key]
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

        public ICollection<string> Keys
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

        public void Add(KeyValuePair<string, SingleTimer> item)
        {
            Add(item.Key, item.Value);
        }

        public SingleTimerLib.SingleTimer AddTimer(string key, SingleTimer value)
        {
            Add(key, value);
            return value;
        }

        public void Add(string key, SingleTimer value)
        {
            timers.Add(key, value);
        }

        public void Clear()
        {
            timers.Clear();
        }

        public bool Contains(KeyValuePair<string, SingleTimer> item)
        {
            return timers.Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return timers.ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, SingleTimer>[] array, int arrayIndex)
        {
            KeyValuePair<string, SingleTimer> item = (KeyValuePair<string, SingleTimer>)timers.ToArray()[arrayIndex];
            array[arrayIndex] = item;
        }

        public IEnumerator<KeyValuePair<string, SingleTimer>> GetEnumerator()
        {
            return timers.GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, SingleTimer> item)
        {
            return Remove(item.Key);
        }

        public bool Remove(string key)
        {
            return timers.Remove(key);
        }

        public bool TryGetValue(string key, out SingleTimer value)
        {
            return timers.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return timers.GetEnumerator();
        }
    }
}
