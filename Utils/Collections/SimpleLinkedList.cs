using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

namespace Utils.Collections
{
    public class SimpleLinkedList<T> : IEnumerable<T>
    {
        private class SimpleLinkedListNode
        {
            public SimpleLinkedListNode Previous { get; set; }
            public SimpleLinkedListNode Next { get; set; }
            public T Value { get; set; }

            public override string ToString()
            {
                return (Value != null) ? Value.ToString() : null;
            }
        }

        private SimpleLinkedListNode _first;
        private SimpleLinkedListNode _last;
        private int _count = 0;
        private int _version = 0;

        public T First
        {
            get { return (_first != null) ? _first.Value : default(T); }
        }
        public T Last
        {
            get { return (_last != null) ? _last.Value : default(T); }
        }

        public int Count
        {
            get { return _count; }
        }

        public SimpleLinkedList()
        {
            Clear();
        }

        public void Clear()
        {
            _first = null;
            _last = null;
            _count = 0;
            _version = 0;
        }

        public void AddFirst(T item)
        {
            var node = new SimpleLinkedListNode() { Value = item };
            node.Next = _first;
            if (_first != null)
            {
                node.Previous = _first.Previous;
                _first.Previous = node;
            }
            _first = node;
            if (_last == null)
                _last = _first;
            _count++;
            _version++;
        }

        public void AddLast(T item)
        {
            var node = new SimpleLinkedListNode() { Value = item };
            node.Previous = _last;
            if (_last != null)
            {
                node.Next = _last.Next;
                _last.Next = node;
            }
            _last = node;
            if (_first == null)
                _first = _last;
            _count++;
            _version++;
        }

        public void RemoveFirst()
        {
            if (_first != null)
            {
                var node = _first;
                _first = node.Next;
                if (_first == null)
                    _last = null;
                RemoveNode(node);
            }
        }
        public void RemoveLast()
        {
            if (_last != null)
            {
                var node = _last;
                _last = node.Previous;
                if (_last == null)
                    _first = null;
                RemoveNode(node);
            }
        }
        private void RemoveNode(SimpleLinkedListNode node)
        {
            if (node != null)
            {
                var prevNode = node.Previous;
                var nextNode = node.Next;

                node.Previous = null;
                node.Next = null;
                if (prevNode != null)
                    prevNode.Next = nextNode;
                if (nextNode != null)
                    nextNode.Previous = prevNode;

                _count--;
                _version++;
            }
        }

        public void AppendList(SimpleLinkedList<T> list)
        {
            if ((list != null) && (list._count > 0))
            {
                if (_last != null)
                {
                    _last.Next = list._first;
                    list._first.Previous = _last;
                }
                else
                {
                    _first = list._first;
                }
                _last = list._last;
                _count += list._count;
                _version++;
            }
        }


        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return new SimpleLinkedList<T>.Enumerator(this);
        }
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return new SimpleLinkedList<T>.Enumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new SimpleLinkedList<T>.Enumerator(this);
        }

        #endregion

        public class Enumerator : IEnumerator<T>, IEnumerator, IDisposable
        {
            private SimpleLinkedList<T> _list;
            private int _version;

            private SimpleLinkedListNode _current;
            private bool _endOfList;
            private bool _startOfList;

            public T Current
            {
                get { return (_current != null) ? _current.Value : default(T); }
            }
            T IEnumerator<T>.Current
            {
                get { return Current; }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }
            public bool EndOfList
            {
                get { return _endOfList; }
            }
            public bool StartOfList
            {
                get { return _startOfList; }
            }

            internal Enumerator(SimpleLinkedList<T> list)
            {
                _list = list;
                _version = _list._version;

                Reset();
            }

            void IDisposable.Dispose()
            {
            }

            public bool MoveNext()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                if (_endOfList)
                    return false;

                _endOfList = (_current == _list._last);
                if (_endOfList)
                    _current = null;
                else
                    _current = (_startOfList ? _list._first : _current.Next);
                _startOfList = false;

                return !_endOfList;
            }
            public bool MovePrevious()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                if (_startOfList)
                    return false;

                _startOfList = (_current == _list._first);
                if (_startOfList)
                    _current = null;
                else
                    _current = (_endOfList ? _list._last : _current.Previous);
                _endOfList = false;

                return !_endOfList;
            }
            public void Reset()
            {
                if (_version != _list._version)
                    throw new InvalidOperationException("Cannot add or remove items with Enumerating list");

                _current = null;
                _endOfList = (_list.Count == 0);
                _startOfList = true;
            }

        }

    }

}
