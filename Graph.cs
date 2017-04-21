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
	[CreateAssetMenu(fileName = "Node Graph", menuName = "Node Graph")]
	[Serializable]
	public class Graph : ScriptableObject
	{
		[SerializeField]
		private List<Node> nodes;
		[SerializeField]
		private SocketBimap links;
		[SerializeField]
		private int nextNodeID;
		[SerializeField]
		private NodeGraphPolicy policy = NodeGraphPolicy.DefaultPolicy;

		#region Properties

		/// <summary>
		/// A list of nodes in this graph.
		/// </summary>
		public List<Node> Nodes
		{
			get
			{
				nodes = nodes ?? new List<Node>();
				return nodes;
			}
			set { nodes = value; }
		}

		/// <summary>
		/// A list of links in this graph.
		/// </summary>
		public SocketBimap Links
		{
			get
			{
				links = links ?? new SocketBimap();
				return links;
			}
			set { links = value; }
		}

		/// <summary>
		/// The next available node ID.
		/// </summary>
		public int NextNodeID
		{
			get { return nextNodeID; }
			set { nextNodeID = value; }
		}

		public NodeGraphPolicy Policy
		{
			get { return policy; }
			set { policy = value; }
		}

		#endregion

		/// <summary>
		/// Calculates the center of the graph, which is the average of all
		/// node positions in the graph.
		/// </summary>
		/// <returns>The center position of the graph.</returns>
		public Vector2 CalculateCenter()
		{
			var nodes = Nodes;
			if (nodes.Count == 0) {
				return new Vector2(0, 0);
			}

			var count = 1;
			var center = nodes[0].Pos;
			for (int i = 1; i < nodes.Count; ++i)
			{
				if (!Util.IsNull(nodes[i]))
				{
					count++;
					center += nodes[i].Pos;
				}
			}
			center /= count;

			return center;
		}

		public override string ToString()
		{
			return string.Format(
				"[Graph \"{0}\"]",
				string.IsNullOrEmpty(name) ? "<i>null</i>" : name
			);
		}
	}
}
