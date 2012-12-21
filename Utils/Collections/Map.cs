using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Utils.Collections
{
    public class Map<TKey, TValue> : IEnumerable<TValue>
    {
        private Dictionary<TKey, TValue> _items = new Dictionary<TKey,TValue>();
        private List<TKey> _sortedKeys = new List<TKey>();
        private int _version = 0;

        public int Count
        {
            get { return _items.Count; }
        }

        public TValue this[TKey key]
        {
            get
            {
                return _items[key];
            }
        }

        public Map()
        {
        }

        public void Clear()
        {
            _items.Clear();
            _sortedKeys.Clear();

            _version++;
        }

        public void Add(TKey key, TValue value)
        {
            _items.Add(key, value);

            int index = _sortedKeys.BinarySearch(key);
            if (index < 0)
            {
                index = ~index;
                _sortedKeys.Insert(index, key);
            }

            _version++;
        }

        public void Remove(TKey key)
        {
            _items.Remove(key);
            _sortedKeys.Remove(key);

            _version++;
        }

        public class Subset : IEnumerable<TValue>
        {
            Map<TKey, TValue> _map;
            int _minIndex;
            int _maxIndex;
            bool _descending;
            public Subset(Map<TKey, TValue> map, int minIndex, int maxIndex, bool descending)
            {
                _map = map;
                _minIndex = minIndex;
                _maxIndex = maxIndex;
                _descending = descending;
            }

            public IEnumerator<TValue> GetEnumerator()
            {
                return new Enumerator(_map, _minIndex, _maxIndex, _descending);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return new Enumerator(_map, _minIndex, _maxIndex, _descending);
            }

            public Subset Descending()
            {
                return new Subset(_map, _minIndex, _maxIndex, true);
            }
        }

        public Subset GetSubset(TKey firstKey, TKey lastKey)
        {
            int firstIndex = _sortedKeys.BinarySearch(firstKey);
            if (firstIndex < 0)
            {
                firstIndex = ~firstIndex;
            }

            int lastIndex = _sortedKeys.BinarySearch(lastKey);
            if (lastIndex < 0)
            {
                lastIndex = ~lastIndex;
                lastIndex--;
            }

            return new Subset(this, firstIndex, lastIndex, false);
        }

        /// <summary>
        /// Returns a 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Subset GetSubsetGreaterThanOrEqual(TKey key)
        {
            int index = _sortedKeys.BinarySearch(key);
            if (index < 0)
            {
                index = ~index;
            }
            return new Subset(this, index, _sortedKeys.Count - 1, false);
        }

        /// <summary>
        /// Returns a 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Subset GetSubsetLessThanOrEqual(TKey key)
        {
            int index = _sortedKeys.BinarySearch(key);
            if (index < 0)
            {
                index = ~index;
                index--;
            }
            return new Subset(this, 0, index, false);
        }

        public Subset Descending()
        {
            return new Subset(this, 0, _sortedKeys.Count - 1, true);
        }


        #region IEnumerable<T> Members
        public IEnumerator<TValue> GetEnumerator()
        {
            return new Enumerator(this, 0, _sortedKeys.Count - 1, false);
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new Enumerator(this, 0, _sortedKeys.Count - 1, false);
        }
        #endregion

        public struct Enumerator : IEnumerator<TValue>, IEnumerator, IDisposable
        {
            private Map<TKey, TValue> _list;
            private int _version;

            private int _index;
            private int _startIndex;
            private int _endIndex;
            private bool _descending;

            private TValue _current;


            TValue IEnumerator<TValue>.Current
            {
                get { return _current; }
            }
            object IEnumerator.Current
            {
                get { return _current; }
            }

            public Enumerator(Map<TKey, TValue> list, int minIndex, int maxIndex, bool descending)
            {
                _list = list;
                _version = list._version;

                _descending = descending;
                if (_descending)
                {
                    _endIndex = minIndex;
                    _startIndex = maxIndex;
                }
                else
                {
                    _startIndex = minIndex;
                    _endIndex = maxIndex;
                }

                _index = _startIndex;
                _current = default(TValue);
            }

            void IDisposable.Dispose()
            {
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                if (_descending)
                {
                    if ((_index >= _endIndex) && (_index >= 0) && (_index < _list._sortedKeys.Count))
                    {
                        var key = _list._sortedKeys[_index--];
                        _current = _list._items[key];
                        return true;
                    }
                    else
                    {
                        _current = default(TValue);
                        _index = _endIndex - 1;
                        return false;
                    }
                }
                else
                {
                    if ((_index <= _endIndex) && (_index >= 0) && (_index < _list._sortedKeys.Count))
                    {
                        var key = _list._sortedKeys[_index++];
                        _current = _list._items[key];
                        return true;
                    }
                    else
                    {
                        _current = default(TValue);
                        _index = _endIndex + 1;
                        return false;
                    }
                }
            }
            void System.Collections.IEnumerator.Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                _index = _startIndex;
                _current = default(TValue);
            }

        }
    }

}
