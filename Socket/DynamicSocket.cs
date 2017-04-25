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
	/// Stores various properties about a socket.
	/// </summary>
	[Serializable]
	public sealed class DynamicSocket
	{
		[SerializeField]
		private string socketName;
		[SerializeField]
		private string description;
		[SerializeField]
		private SocketFlags flags;
		[SerializeField]
		private int width = SocketAttribute.DEFAULT_WIDTH;
		[SerializeField]
		private DynamicValue value;

		#region Properties

		/// <summary>
		/// The name of the socket.
		/// </summary>
		public string SocketName
		{
			get { return socketName; }
			set { socketName = value; }
		}

		/// <summary>
		/// The description of the socket.
		/// </summary>
		public string Description
		{
			get { return description; }
			set { description = value; }
		}

		/// <summary>
		/// The type of the socket.
		/// </summary>
		public Type SocketType
		{
			get { return value.Type; }
			set { this.value.Type = value; }
		}

		/// <summary>
		/// True if this socket's value is editable.
		/// </summary>
		public SocketFlags Flags
		{
			get { return flags; }
			set { flags = value; }
		}

		/// <summary>
		/// The width of this socket in the node graph UI.
		/// </summary>
		public int Width
		{
			get { return width; }
			set { width = value; }
		}

		/// <summary>
		/// The value of this socket.
		/// </summary>
		public object SocketValue
		{
			get { return value.Value; }
			set { this.value.Value = value; }
		}

		#endregion

		/// <summary>
		/// Creates a new SocketDefinition.
		/// </summary>
		/// <param name="type">The type of the socket.</param>
		/// <param name="name">The name of the socket.</param>
		/// <param name="flags">The socket flags.</param>
		/// <param name="allowMultipleLinks">
		/// True if this socket allows multiple links.
		/// </param>
		/// <param name="width">
		/// The width of this socket in the node graph UI.
		/// </param>
		public DynamicSocket(Type type, string name, string description = null,
			SocketFlags flags = 0, int width = SocketAttribute.DEFAULT_WIDTH)
		{
			value = new DynamicValue();
			SocketType = type;
			SocketName = name;
			Description = description;
			Flags = flags;
			Width = width;
		}
	}
}
