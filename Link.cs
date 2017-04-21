/* Unity3D Node Graph Framework
Copyright (c) 2017 Ava Pek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// Represents a signal from one socket to another.
	/// </summary>
	[Serializable]
	public class Link
	{
		[SerializeField]
		private Socket fromSocket;
		[SerializeField]
		private Socket toSocket;

		#region Properties

		/// <summary>
		/// The socket the signal is from.
		/// </summary>
		public Socket FromSocket
		{
			get { return fromSocket; }
			set { fromSocket = value; }
		}

		/// <summary>
		/// The socket the signal is going to.
		/// </summary>
		public Socket ToSocket
		{
			get { return toSocket; }
			set { toSocket = value; }
		}

		#endregion

		public Link(Graph graph, Socket fromSocket, Socket toSocket)
		{
			var from = fromSocket.IsInput(graph) ? toSocket : fromSocket;
			var to = fromSocket.IsInput(graph) ? fromSocket : toSocket;

			if (from == to)
			{
				var msg = "Sockets in a link cannot have the same input state";
				throw new ArgumentException(msg);
			}

			this.fromSocket = fromSocket;
			this.toSocket = toSocket;
		}

		#region Equality

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;

			return IsEqual((Link)obj);
		}

		public bool Equals(Link other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return IsEqual(other);
		}

		public override int GetHashCode()
		{
			return Util.GetHashCode(fromSocket, toSocket);
		}

		public static bool operator ==(Link l, Link r)
		{
			if (ReferenceEquals(l, r))
			{
				return true;
			}

			if (ReferenceEquals(null, l))
			{
				return false;
			}

			return (l.Equals(r));
		}

		public static bool operator !=(Link l, Link r)
		{
			return !(l == r);
		}

		private bool IsEqual(Link other)
		{
			return int.Equals(fromSocket, other.fromSocket)
				&& string.Equals(toSocket, other.toSocket);
		}

		#endregion

		public override string ToString()
		{
			return string.Format(
				"[{0} -> {1}]",
				fromSocket,
				toSocket
			);
		}
	}
}
