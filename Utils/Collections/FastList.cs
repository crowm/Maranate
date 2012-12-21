using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Utils.Collections
{
    public class FastList<T> : IEnumerable<T>
    {
        private T[] _items;
        private int _count;
        private int _version;

        private void EnsureCapacity(int capacity)
        {
            if ((_items == null) || (_items.Length < capacity))
            {
                var newItems = new T[capacity];
                if ((_items != null) && (_count > 0))
                {
                    Array.Copy(_items, newItems, _count);
                }
                _items = newItems;
            }
        }

        public int Count
        {
            get { return _count; }
        }

        public T this[int index]
        {
            get
            {
                if ((index < 0) || (index >= _count))
                    throw new IndexOutOfRangeException();
                return _items[index];
            }
        }

        public FastList()
        {
            _items = new T[0];
            _count = 0;
            _version = 0;
        }

        public void Clear()
        {
            _items = new T[0];
            _count = 0;
            _version++;
        }

        public void Add(T item)
        {
            var length = _items.Length;
            if (_count + 1 > length)
            {
                var expand = (length < 1000) ? (length + 1) * 4 : 1000;
                EnsureCapacity(length + expand);
            }

            _items[_count++] = item;
            _version++;
        }

        public void AddRange(T[] items)
        {
            AddRange(items, items.Length);
        }
        public void AddRange(FastList<T> list)
        {
            AddRange(list._items, list._count);
        }
        public void AddRange(T[] items, int count)
        {
            if ((count < 0) || (count > items.Length))
                throw new ArgumentOutOfRangeException();

            if (count == 0)
                return;

            var length = _items.Length;
            if (_count + count > length)
            {
                var expand = (length < 1000) ? (length + 1) * 4 : 1000;
                EnsureCapacity(length + count + expand);
            }

            Array.Copy(items, 0, _items, _count, count);
            _count += count;
            _version++;
        }

        public void RemoveAt(int index)
        {
            if (index > _count)
                throw new ArgumentOutOfRangeException();

            _count--;
            if (index < _count)
                Array.Copy(_items, index + 1, _items, index, _count - index);
            _items[_count] = default(T);
            _version++;
        }

        #region IEnumerable<T> Members
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new FastList<T>.Enumerator(this);
        }
        #endregion

        #region IEnumerable Members
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return new FastList<T>.Enumerator(this);
        }
        #endregion

        public struct Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private FastList<T> _list;
            private int _version;

            private int _index;
            private T _current;

            T IEnumerator<T>.Current
            {
                get { return _current; }
            }
            object IEnumerator.Current
            {
                get { return _current; }
            }

            internal Enumerator(FastList<T> list)
            {
                _list = list;
                _version = list._version;

                _index = 0;
                _current = default(T);
            }

            void IDisposable.Dispose()
            {
            }

            bool System.Collections.IEnumerator.MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                if (_index < _list._count)
                {
                    _current = _list._items[_index++];
                    return true;
                }
                else
                {
                    _current = default(T);
                    _index = _list._count + 1;
                    return false;
                }
            }
            void System.Collections.IEnumerator.Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                _index = 0;
                _current = default(T);
            }

        }
    }

}
