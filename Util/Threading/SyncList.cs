using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A wrapper for a List to make it thread-safe.
	/// </summary>
	/// <typeparam name="T">
	/// The type parameter of the wrapped IList.
	/// </typeparam>
	public class SyncList<T> : IList<T>
	{
		private readonly List<T> list;

		public SyncList()
		{
			list = new List<T>();
		}

		public SyncList(List<T> list)
		{
			this.list = list;
		}

		public List<T> ToList()
		{
			lock (this) { return new List<T>(list); }
		}

		#region ICollection

		public int Count
		{
			get { lock (this) { return list.Count; } }
		}

		public bool IsReadOnly
		{
			get { return false; }
		}

		public void Add(T item)
		{
			lock (this) { list.Add(item); }
		}

		public void Clear()
		{
			lock (this) { list.Clear(); }
		}

		public bool Contains(T item)
		{
			lock (this) { return list.Contains(item); }
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			lock (this) { list.CopyTo(array, arrayIndex); }
		}

		public bool Remove(T item)
		{
			lock (this) { return list.Remove(item); }
		}

		#endregion

		#region IList

		public T this[int index]
		{
			get { lock (this) { return list[index]; } }
			set { lock (this) { list[index] = value; } }
		}

		public int IndexOf(T item)
		{
			lock (this) { return list.IndexOf(item); }
		}

		public void Insert(int index, T item)
		{
			lock (this) { list.Insert(index, item); }
		}

		public void RemoveAt(int index)
		{
			lock (this) { list.RemoveAt(index); }
		}

		#endregion

		#region IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			return new SyncEnumerator<T>(list.GetEnumerator(), this);
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return new SyncEnumerator<T>(list.GetEnumerator(), this);
		}

		#endregion
	}
}
