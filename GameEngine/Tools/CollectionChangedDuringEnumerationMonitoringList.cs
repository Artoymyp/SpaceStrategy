using System;
using System.Collections;
using System.Collections.Generic;

namespace SpaceStrategy.Tools
{
	class CollectionChangedDuringEnumerationMonitoringList<T> : IList<T>
	{
		readonly List<T> _list = new List<T>();
		NotifyingEnumeratorWrapper _enumerator;

		public IEnumerator<T> GetEnumerator()
		{
			_enumerator = new NotifyingEnumeratorWrapper(_list.GetEnumerator());
			return _enumerator;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			_enumerator =  new NotifyingEnumeratorWrapper(((IEnumerable)_list).GetEnumerator());
			return _enumerator;
		}

		public void Add(T item)
		{
			OnCollectionChangedDuringEnumeration();
			_list.Add(item);
		}

		public void Clear()
		{
			OnCollectionChangedDuringEnumeration();
			_list.Clear();
		}

		public bool Contains(T item)
		{
			return _list.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_list.CopyTo(array, arrayIndex);
		}

		public bool Remove(T item)
		{
			OnCollectionChangedDuringEnumeration();
			return _list.Remove(item);
		}

		public int Count
		{
			get { return _list.Count; }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public int IndexOf(T item)
		{
			return _list.IndexOf(item);
		}

		public void Insert(int index, T item)
		{
			OnCollectionChangedDuringEnumeration();
			_list.Insert(index, item);
		}

		public void RemoveAt(int index)
		{
			OnCollectionChangedDuringEnumeration();
			_list.RemoveAt(index);
		}

		public void RemoveAll(Predicate<T> predicate)
		{
			_list.RemoveAll(predicate);
		}

		public T this[int index]
		{
			get { return _list[index]; }
			set
			{
				OnCollectionChangedDuringEnumeration();
				_list[index] = value;
			}
		}

		public event EventHandler CollectionChangedDuringEnumeration;

		protected virtual void OnCollectionChangedDuringEnumeration()
		{
			if (_enumerator != null) {
				if (_enumerator.IsEnumerating) {
					CollectionChangedDuringEnumeration?.Invoke(this, EventArgs.Empty);
				}
			}
		}

		public class NotifyingEnumeratorWrapper : IEnumerator<T>, IEnumerator
		{
			readonly IEnumerator _enumerator;
			readonly IEnumerator<T> _typedEnumerator;

			public NotifyingEnumeratorWrapper(IEnumerator<T> enumerator)
			{
				_enumerator = enumerator;
				_typedEnumerator = enumerator;
			}

			public NotifyingEnumeratorWrapper(IEnumerator enumerator)
			{
				_enumerator = enumerator;
				_typedEnumerator = enumerator as IEnumerator<T>;
			}

			public bool IsEnumerating { get; private set; }

			public void Dispose()
			{
				IsEnumerating = false;
				_typedEnumerator?.Dispose();
			}

			public bool MoveNext()
			{
				IsEnumerating = true;
				return _enumerator.MoveNext();
			}

			public void Reset()
			{
				_enumerator.Reset();
			}

			public T Current
			{
				get
				{
					return _typedEnumerator != null ? _typedEnumerator.Current : default;
				}
			}

			object IEnumerator.Current
			{
				get { return _enumerator.Current; }
			}
		}
	}
}