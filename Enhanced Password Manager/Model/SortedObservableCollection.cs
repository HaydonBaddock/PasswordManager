using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Enhanced_Password_Manager.Model
{
	/// <summary>
	/// An onservable collection that is always sorted.
	/// </summary>
	/// <typeparam name="T">Object type that this collection holds</typeparam>
	[Serializable]
	public class SortedObservableCollection<T> : ICollection<T>, INotifyCollectionChanged where T : IComparable, INotifyPropertyChanged
	{
		private readonly List<T> _items = new List<T>();
		public IList<T> Items => _items.AsReadOnly();

		public int Count => _items.Count;

		public bool IsReadOnly => Items.IsReadOnly;

		public void ElementPropertyChanged(object item, PropertyChangedEventArgs args)
		{
			_items.Sort();
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public IEnumerator<T> GetEnumerator()
		{
			return _items.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(T item)
		{
			if (_items.Contains(item)) return;
			int i;
			for (i = 0; i < Count; i++)
				if (Math.Sign(_items[i].CompareTo(item)) >= 0) break;
			var moved = _items.GetRange(i, Count - i);
			item.PropertyChanged += ElementPropertyChanged;
			_items.Insert(i, item);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item));
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, moved, i + 1, i));
		}

		public void AddAll(ICollection<T> items)
		{
			foreach (var item in items)
				Add(item);
		}
		
		public void Clear()
		{
			_items.Clear();
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
		}

		public bool Contains(T item)
		{
			return _items.Contains(item);
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			_items.CopyTo(array, arrayIndex);
		}

		public int IndexOf(T item)
		{
			return _items.IndexOf(item);
		}

		public bool Remove(T item)
		{
			if (!_items.Contains(item)) return false;
			var index = _items.IndexOf(item);
			_items.Remove(item);
			NotifyCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, index));
			return true;
		}

		[field:NonSerialized]
		public event NotifyCollectionChangedEventHandler CollectionChanged;
		private void NotifyCollectionChanged(NotifyCollectionChangedEventArgs args)
		{
			CollectionChanged?.Invoke(this, args);
		}
	}
}
