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
using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// Represents a result in the search panel that, when picked, will add
	/// a new node to the graph.
	/// </summary>
	public abstract class SearchResult
	{
		/// <summary>
		/// The string to display in the search panel.
		/// </summary>
		public abstract string Label { get; }

		/// <summary>
		/// Adds the node represented by this result to a graph at the
		/// specified position.
		/// </summary>
		/// <param name="graph">The graph to add the new node to.</param>
		/// <param name="spawnPosition">The position of the new node.</param>
		public abstract void Pick(Graph graph, Vector2 spawnPosition);

		/// <summary>
		/// Utility method to create a new Dynamic Node.
		/// </summary>
		/// <param name="name">The name of the Node.</param>
		/// <param name="spawnPosition">The position of the new node.</param>
		/// <returns></returns>
		protected DynamicNode CreateNode(string name, Vector2 spawnPosition)
		{
			var node = ScriptableObject.CreateInstance<DynamicNode>();
			node.DisplayName = name;
			node.XPos = Mathf.FloorToInt(spawnPosition.x);
			node.YPos = Mathf.FloorToInt(spawnPosition.y);

			return node;
		}

		/// <summary>
		/// Adds a node to a graph.
		/// </summary>
		/// <param name="graph">The graph to add the new node to.</param>
		/// <param name="node">The node to add.</param>
		protected void AddNode(Graph graph, Node node)
		{
			node.ID = graph.NextNodeID++;

			Undo.RecordObject(graph, null);

			// Check if the graph is a persistent asset or a scene asset
			var path = AssetDatabase.GetAssetPath(graph.GetInstanceID());
			if (!string.IsNullOrEmpty(path))
			{
				AssetDatabase.AddObjectToAsset(node, graph);
			}
			graph.Nodes.Add(node);

			EditorUtility.SetDirty(graph);
		}
	}
}
