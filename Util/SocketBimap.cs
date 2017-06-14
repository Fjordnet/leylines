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
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A bi-directional mapping of sockets.
	/// </summary>
	[Serializable]
	public class SocketBimap : IEnumerable<Link>
	{
		[SerializeField]
		private List<Link> links = new List<Link>();

		#region Operations

		/// <summary>
		/// Adds a new link specified by the two sockets
		/// </summary>
		/// <param name="graph">The graph these sockets are in.</param>
		/// <param name="a">The first socket in the link.</param>
		/// <param name="b">The second socket in the link.</param>
		/// <returns>True if the link did not exist and was added.</returns>
		public bool Add(Graph graph, Socket a, Socket b)
		{
			var from = a.IsInput(graph) ? b : a;
			var to = a.IsInput(graph) ? a : b;

			if (from == to) {
				return false;
			}

			InitCache();

			var link = new Link(graph, from, to);
			if (fromToCache.ContainsKey(from))
			{
				if (!fromToCache[from].Contains(to))
				{
					links.Add(link);
					fromToCache[from].Add(to);
					return true;
				}
			}
			else
			{
				links.Add(link);
				fromToCache[from] = new List<Socket>() { to };
				return true;
			}
			return false;
		}

		/// <summary>
		/// Removes a link from the bimap.
		/// </summary>
		/// <param name="a">The first socket in the link.</param>
		/// <param name="b">The second socket in the link.</param>
		/// <returns>True if that link existed and was removed.</returns>
		public bool Remove(Graph graph, Socket a, Socket b)
		{
			var from = a.IsInput(graph) ? b : a;
			var to = a.IsInput(graph) ? a : b;

			if (from == to)
			{
				return false;
			}

			// Update cache
			InitCache();
			if (fromToCache.ContainsKey(a))
			{
				fromToCache[a].Remove(b);
				if (fromToCache[a].Count == 0)
				{
					fromToCache.Remove(a);
				}
			}
			if (toFromCache.ContainsKey(b))
			{
				toFromCache[b].Remove(a);
				if (toFromCache[b].Count == 0)
				{
					toFromCache.Remove(b);
				}
			}

			// Update serialized data
			return links.Remove(new Link(graph, from, to));
		}

		/// <summary>
		/// Removes all links that use the specified socket.
		/// </summary>
		/// <param name="socket">The socket to remove links to.</param>
		/// <returns>The number of links removed.</returns>
		public int RemoveAllWith(Socket socket)
		{
			// Update cache
			InitCache();
			if (fromToCache.ContainsKey(socket))
			{
				fromToCache.Remove(socket);
			}
			if (toFromCache.ContainsKey(socket))
			{
				toFromCache.Remove(socket);
			}

			// Update serialized data
			return links.RemoveAll(
				t => { return t.FromSocket == socket || t.ToSocket == socket; }
			);
		}

		/// <summary>
		/// Removes all links that use the specified node.
		/// </summary>
		/// <param name="node">The node to remove links to.</param>
		/// <returns>The number of links removed.</returns>
		public int RemoveAllWith(Node node)
		{
			// Update cache
			InitCache();
			var toRemove = new List<Socket>();
			foreach (var socket in fromToCache.Keys)
			{
				if (socket.NodeID == node.ID)
				{
					toRemove.Add(socket);
				}
			}
			foreach(var socket in toRemove)
			{
				fromToCache.Remove(socket);
			}
			toRemove = new List<Socket>();
			foreach (var socket in toFromCache.Keys)
			{
				if (socket.NodeID == node.ID)
				{
					toRemove.Add(socket);
				}
			}
			foreach (var socket in toRemove)
			{
				toFromCache.Remove(socket);
			}

			// Update serialized data
			return links.RemoveAll(
				t =>
				{
					return t.FromSocket.NodeID == node.ID
						|| t.ToSocket.NodeID == node.ID;
				}
			);
		}

		#endregion

		#region Checks

		/// <summary>
		/// Returns true if the link specified by the two sockets exists.
		/// </summary>
		/// <param name="graph">The graph these sockets are in.</param>
		/// <param name="a">The first socket in the link.</param>
		/// <param name="b">The second socket in the link.</param>
		/// <returns>True if that link specified by the two sockets exists.</returns>
		public bool Contains(Graph graph, Socket a, Socket b)
		{
			InitCache();

			var from = a.IsInput(graph) ? b : a;
			var to = a.IsInput(graph) ? a : b;

			if (!fromToCache.ContainsKey(from))
			{
				return false;
			}

			return fromToCache[from].Contains(to);
		}

		/// <summary>
		/// Return a list of sockets that have the specified socket as a source.
		/// </summary>
		/// <param name="socket">The socket to use as the source.</param>
		/// <returns>
		/// A list of sockets that have a specified socket as a source.
		/// </returns>
		public Socket[] HasSocketAsSource(Socket socket)
		{
			InitCache();

			if (!fromToCache.ContainsKey(socket))
			{
				return new Socket[0];
			}
			return fromToCache[socket].ToArray();
		}

		/// <summary>
		/// Return a list of sockets that have the specified socket as a
		/// destination.
		/// </summary>
		/// <param name="socket">The socket to use as the destination.</param>
		/// <returns>
		/// A list of sockets that have a specified socket as a source.
		/// </returns>
		public Socket[] HasSocketAsDestination(Socket socket)
		{
			InitCache();

			if (!toFromCache.ContainsKey(socket))
			{
				return new Socket[0];
			}
			return toFromCache[socket].ToArray();
		}

		/// <summary>
		/// Returns true if the specified socket is the destination for any
		/// links.
		/// </summary>
		/// <param name="socket">The socket to check.</param>
		/// <returns>True if the specified socket is linked to.</returns>
		public bool IsSocketLinkedTo(Socket socket)
		{
			InitCache();

			if (!toFromCache.ContainsKey(socket))
			{
				return false;
			}

			return true;
		}

		/// <summary>
		/// Returns true if the specified socket is the source for any
		/// links.
		/// </summary>
		/// <param name="socket">The socket to check.</param>
		/// <returns>True if the specified socket is linked from.</returns>
		public bool IsSocketLinkedFrom(Socket socket)
		{
			InitCache();

			if (!fromToCache.ContainsKey(socket))
			{
				return false;
			}

			return true;
		}

		#endregion

		IEnumerator IEnumerable.GetEnumerator()
		{
			return links.GetEnumerator();
		}

		public IEnumerator<Link> GetEnumerator()
		{
			return links.GetEnumerator();
		}

		#region Util

		private Dictionary<Socket, List<Socket>> fromToCache;
		private Dictionary<Socket, List<Socket>> toFromCache;
		private void InitCache()
		{
			if (fromToCache == null)
			{
				fromToCache = new Dictionary<Socket, List<Socket>>();

				foreach (var link in links)
				{
					if (fromToCache.ContainsKey(link.FromSocket))
					{
						fromToCache[link.FromSocket].Add(link.ToSocket);
					}
					else
					{
						fromToCache[link.FromSocket] =
							new List<Socket>() { link.ToSocket };
					}
				}
			}
			if (toFromCache == null)
			{
				toFromCache = new Dictionary<Socket, List<Socket>>();

				foreach (var link in links)
				{
					if (toFromCache.ContainsKey(link.ToSocket))
					{
						toFromCache[link.ToSocket].Add(link.FromSocket);
					}
					else
					{
						toFromCache[link.ToSocket] =
							new List<Socket>() { link.FromSocket };
					}
				}
			}
		}

		#endregion
	}
}
