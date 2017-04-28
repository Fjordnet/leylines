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
	/// A socket is a representation of a member field on a node.
	/// </summary>
	[Serializable]
	public sealed class Socket
	{
		[SerializeField]
		private int nodeID;
		[SerializeField]
		private string fieldName;

		#region Properties

		/// <summary>
		/// The ID of the node this socket is on.
		/// </summary>
		public int NodeID
		{
			get { return nodeID; }
		}

		/// <summary>
		/// The name of the member this socket represents.
		/// </summary>
		public string FieldName
		{
			get { return fieldName; }
		}

		#endregion

		public Socket(int nodeID, string fieldName)
		{
			this.nodeID = nodeID;
			this.fieldName = fieldName;
		}

		/// <summary>
		/// Returns the node this socket is on.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>The node this socket is on.</returns>
		public Node GetNode(Graph graph)
		{
			var node = graph.Nodes.Find(
				n => { return n != null && n.ID == NodeID; }
			);

			if (Util.IsNull(node))
			{
				var msg = "Node " + nodeID + " does not exist";
				throw new InvalidOperationException(msg);
			}
			return node;
		}

		#region Getters/Setters

		/// <summary>
		/// Returns the description of this socket.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>The description of this socket.</returns>
		public string GetDescription(Graph graph)
		{
			return GetNode(graph).GetSocketDescription(this);
		}

		/// <summary>
		/// Returns the display name of this socket.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>The display name of this socket.</returns>
		public string GetDisplayName(Graph graph)
		{
			return GetNode(graph).GetSocketDisplayName(this);
		}

		/// <summary>
		/// Returns the flags for this socket.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>The flags for this socket.</returns>
		public SocketFlags GetFlags(Graph graph)
		{
			return GetNode(graph).GetSocketFlags(this);
		}

		/// <summary>
		/// Returns the type of this socket.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>The type of this socket.</returns>
		public Type GetSocketType(Graph graph)
		{
			return GetNode(graph).GetSocketType(this);
		}

		/// <summary>
		/// Returns the value of this socket.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>The value of this socket.</returns>
		public object GetValue(Graph graph)
		{
			return GetNode(graph).GetSocketValue(this);
		}

		/// <summary>
		/// Returns true if this socket is an input.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <returns>True if this socket is an input.</returns>
		public bool IsInput(Graph graph)
		{
			return GetNode(graph).IsSocketInput(this);
		}

		/// <summary>
		/// Sets the value of a socket on this node.
		/// </summary>
		/// <param name="graph">The graph this socket is in.</param>
		/// <param name="value">The value to set.</param>
		public void SetValue(Graph graph, object value)
		{
			GetNode(graph).SetSocketValue(this, value);
		}

		#endregion

		#region Equality

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;

			return IsEqual((Socket)obj);
		}

		public bool Equals(Socket other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;

			return IsEqual(other);
		}

		public override int GetHashCode()
		{
			return Util.GetHashCode(nodeID, fieldName);
		}

		public static bool operator ==(Socket l, Socket r)
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

		public static bool operator !=(Socket l, Socket r)
		{
			return !(l == r);
		}

		private bool IsEqual(Socket other)
		{
			return int.Equals(nodeID, other.nodeID)
				&& string.Equals(fieldName, other.fieldName);
		}

		#endregion

		public override string ToString()
		{
			return string.Format(
				"[Socket {0}, {1}]",
				nodeID,
				fieldName
			);
		}
	}
}