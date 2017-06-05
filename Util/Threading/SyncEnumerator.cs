using System.Collections;
using System.Collections.Generic;
using System.Threading;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A wrapper for an IEnumerator to make it thread-safe.
	/// </summary>
	/// <typeparam name="T">
	/// The type parameter of the wrapped IEnumerator.
	/// </typeparam>
	public class SyncEnumerator<T> : IEnumerator<T>
	{
		private readonly IEnumerator<T> iter;
		private readonly object locker;

		public SyncEnumerator(IEnumerator<T> iter, object locker)
		{
			this.iter = iter;
			this.locker = locker;
			Monitor.Enter(locker);
		}

		#region IDispose

		public void Dispose()
		{
			Monitor.Exit(locker);
		}

		#endregion

		#region IEnumerator

		public T Current
		{
			get { return iter.Current; }
		}

		object IEnumerator.Current
		{
			get { return iter.Current; }
		}

		public bool MoveNext()
		{
			return iter.MoveNext();
		}

		public void Reset()
		{
			iter.Reset();
		}

		#endregion
	}
}
