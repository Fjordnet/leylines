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
using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A Node is an item in a node graph that takes inputs and returns
	/// outputs.
	/// </summary>
	[Serializable]
	public abstract class Node : ScriptableObject
	{
		[SerializeField]
		private int id;
		[SerializeField]
		private int xPos;
		[SerializeField]
		private int yPos;

		#region Properties

		/// <summary>
		/// The ID of the node.
		/// </summary>
		public int ID
		{
			get { return id; }
			set { id = value; }
		}

		/// <summary>
		/// The X position of the node.
		/// </summary>
		public int XPos
		{
			get { return xPos; }
			set { xPos = value; }
		}

		/// <summary>
		/// The Y position of the node.
		/// </summary>
		public int YPos
		{
			get { return yPos; }
			set { yPos = value; }
		}

		/// <summary>
		/// The position of the node.
		/// </summary>
		public Vector2 Pos
		{
			get { return new Vector2(xPos, yPos); }
			set
			{
				xPos = (int)Math.Round(value.x);
				yPos = (int)Math.Round(value.y);
			}
		}

		/// <summary>
		/// The display name of this node.
		/// </summary>
		public abstract string DisplayName { get; set; }

		#endregion

		#region Node Logic

		/// <summary>
		/// Executes this node.
		/// </summary>
		/// <param name="from">The socket the signal comes from.</param>
		/// <param name="to">The socket the signal is sent to.</param>
		/// <param name="scope">The in-scope variables.</param>
		/// <returns>A yield.</returns>
		public virtual IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			yield return null;
		}

		/// <summary>
		/// Evaluates this node.
		/// </summary>
		/// <param name="scope">The in-scope variables.</param>
		/// <returns>The execution socket to trigger.</returns>
		public virtual void Eval(GraphScope scope) { }

		#endregion

		#region Socket Getters

		/// <summary>
		/// Returns a socket with the specified name.
		/// </summary>
		/// <param name="name">The name of the socket to get.</param>
		/// <returns>A socket with the specified name.</returns>
		public abstract Socket GetSocket(string name);

		/// <summary>
		/// Returns all of the sockets on this node.
		/// </summary>
		/// <returns>All of the sockets on this node.</returns>
		public abstract Socket[] GetSockets();

		/// <summary>
		/// Returns all of the input sockets on this node.
		/// </summary>
		/// <returns>All of the input sockets on this node.</returns>
		public abstract Socket[] GetInputSockets();

		/// <summary>
		/// Returns all of the output sockets on this node.
		/// </summary>
		/// <returns>All of the output sockets on this node.</returns>
		public abstract Socket[] GetOutputSockets();

		#endregion

		#region Socket Properties

		/// <summary>
		/// Returns the display name of a socket on this node.
		/// </summary>
		/// <param name="name">
		/// The name of the socket to get the value of.
		/// </param>
		/// <returns>The display name of the socket.</returns>
		public abstract string GetSocketDisplayName(string name);

		/// <summary>
		/// Returns the display name of a socket on this node.
		/// </summary>
		/// <param name="socket">The socket to get the display name of.</param>
		/// <returns>The display name of the socket.</returns>
		public string GetSocketDisplayName(Socket socket)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			return GetSocketDisplayName(socket.FieldName);
		}

		/// <summary>
		/// Returns the flags of a socket on this node.
		/// </summary>
		/// <param name="name">The name of the socket.</param>
		/// <returns>The socket flags.</returns>
		public abstract SocketFlags GetSocketFlags(string name);

		/// <summary>
		/// Returns the flags of a socket on this node.
		/// </summary>
		/// <param name="name">The socket to check.</param>
		/// <returns>The socket flags.</returns>
		public SocketFlags GetSocketFlags(Socket socket)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			return GetSocketFlags(socket.FieldName);
		}

		/// <summary>
		/// Returns the type of the socket.
		/// </summary>
		/// <param name="name">The name of the socket.</param>
		/// <returns>The type of the socket.</returns>
		public abstract Type GetSocketType(string name);

		/// <summary>
		/// Returns the type of the socket.
		/// </summary>
		/// <param name="socket">The socket to get the type of.</param>
		/// <returns>The type of the socket.</returns>
		public Type GetSocketType(Socket socket)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			return GetSocketType(socket.FieldName);
		}

		/// <summary>
		/// Returns the value of a socket on this node.
		/// </summary>
		/// <param name="name">
		/// The name of the socket to get the value of.
		/// </param>
		/// <returns>The value of the socket.</returns>
		public abstract object GetSocketValue(string name);

		/// <summary>
		/// Returns the value of a socket on this node.
		/// </summary>
		/// <param name="socket">The socket to get the value of.</param>
		/// <returns>The value of the socket.</returns>
		public object GetSocketValue(Socket socket)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			return GetSocketValue(socket.FieldName);
		}

		/// <summary>
		/// Returns the width of a socket in the UI.
		/// </summary>
		/// <param name="name">
		/// The name of the socket to get the width of.
		/// </param>
		/// <returns>The width of the socket.</returns>
		public abstract int GetSocketWidth(string name);

		/// <summary>
		/// Returns the width of a socket in the UI.
		/// </summary>
		/// <param name="socket">The socket to get the width of.</param>
		/// <returns>The width of the socket.</returns>
		public int GetSocketWidth(Socket socket)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			return GetSocketWidth(socket.FieldName);
		}

		/// <summary>
		/// Returns true if the specified socket is an input socket.
		/// </summary>
		/// <param name="name">The name of the socket.</param>
		/// <returns>True if the specified socket is an input socket.</returns>
		public abstract bool IsSocketInput(string name);

		/// <summary>
		/// Returns true if the specified socket is an input socket.
		/// </summary>
		/// <param name="socket">The socket to check.</param>
		/// <returns>True if the specified socket is an input socket.</returns>
		public bool IsSocketInput(Socket socket)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			return IsSocketInput(socket.FieldName);
		}

		/// <summary>
		/// Sets the value of a socket on this node.
		/// </summary>
		/// <param name="name">
		/// The name of the socket to get the value of.
		/// </param>
		/// <param name="value">The value to set.</param>
		public abstract void SetSocketValue(string name, object value);

		/// <summary>
		/// Sets the value of a socket on this node.
		/// </summary>
		/// <param name="socket">The socket to set the value of.</param>
		/// <param name="value">The value to set.</param>
		public void SetSocketValue(Socket socket, object value)
		{
			if (socket.NodeID != id) {
				throw new ArgumentException
					("Specified socket has the wrong node ID!");
			}
			SetSocketValue(socket.FieldName, value);
		}

		#endregion

		#region MonoBehaviour

		void OnEnable()
		{
			hideFlags = HideFlags.HideInHierarchy | HideFlags.HideInInspector;
		}

		#endregion

		public override string ToString()
		{
			return string.Format(
				"[{0} ({1},{2})]",
				id, xPos, yPos
			);
		}
	}
}
