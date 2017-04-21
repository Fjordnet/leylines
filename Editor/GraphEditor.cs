﻿/* Unity3D Node Graph Framework
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
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[Serializable]
	public class GraphEditor : EditorWindow
	{
		[SerializeField]
		private Graph graph;

		#region Properties

		public Graph Graph
		{
			get { return graph; }
			set { graph = value; }
		}

		public object Target { get; set; }
		public Vector2 Offset { get; set; }
		public int Snap { get { return snap ? GRID_CELL_SIZE : 0; } }
		public Vector2 GraphPosition { get; private set; }

		#endregion

		public const int GRAPH_PADDING = 2;
		public const int GRID_CELL_SIZE = 20;
		public const int TOOLBAR_HEIGHT = 23;

		private bool snap = false;
		private SearchEditor search = new SearchEditor(new Vector2(500, 300));

		public Dictionary<Socket, Rect> rectCache;

		#region Launchers

		[MenuItem("Window/Node Graph Editor")]
		private static void Launch()
		{
			Launch(null);
		}

		public static void Launch(Graph graph)
		{
			var window = GetWindow<GraphEditor>(typeof(SceneView));

			window.titleContent = new GUIContent("Node Graph");
			window.Graph = graph;
			window.CenterViewOn(Vector2.zero);
			window.Show();
		}

		[UnityEditor.Callbacks.OnOpenAsset(1)]
		public static bool OnOpenAsset(int instanceID, int line)
		{
			var obj = EditorUtility.InstanceIDToObject(instanceID);

			if (obj is Graph)
			{
				Launch((Graph)obj);
				return true;
			}
			return false;
		}

		#endregion

		void OnGUI()
		{
			rectCache = rectCache ?? new Dictionary<Socket, Rect>();
			rectCache.Clear();

			antiAlias = 2;
			autoRepaintOnSceneChange = true;
			wantsMouseMove = true;

			Offset = new Vector2(
				Mathf.RoundToInt(Offset.x),
				Mathf.RoundToInt(Offset.y)
			);

			var graphRect = GetGraphRect();

			var mousePos = Event.current.mousePosition;
			GraphPosition = new Vector2(
				mousePos.x - graphRect.xMin - Offset.x,
				-mousePos.y + graphRect.yMin + Offset.y
			);

			// Top Toolbar
			var toolbarHeight = Vector2.up * TOOLBAR_HEIGHT;

			var topToolbarRect = new Rect(position);
			topToolbarRect.position = new Vector2(4, 4);
			topToolbarRect.size -= new Vector2(10, 0);
			topToolbarRect.height = toolbarHeight.y;

			GUILayout.BeginArea(topToolbarRect);
			GUILayout.BeginHorizontal();

			if (GUILayout.Button("Save", GUILayout.MaxHeight(15)))
			{
				AssetDatabase.SaveAssets();
			}

			var oldGraph = Graph;
			Graph = EditorGUILayout.ObjectField(Graph, typeof(Graph), false) as Graph;
			if (Graph != oldGraph)
			{
				if (!Util.IsNull(Graph))
				{
					CenterViewOn(Graph.CalculateCenter());
				}
				else
				{
					CenterViewOn(Vector2.zero);
				}
			}
			GUILayout.FlexibleSpace();

			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			// Bottom Toolbar
			var bottomToolbarRect = new Rect(topToolbarRect);
			bottomToolbarRect.y = position.size.y - toolbarHeight.y + 2;

			GUILayout.BeginArea(bottomToolbarRect);
			GUILayout.BeginHorizontal();

			snap = GUILayout.Toggle(snap, "Snap", GUI.skin.button);
			if (GUILayout.Button("Center View"))
			{
				if (Graph != null)
				{
					CenterViewOn(Graph.CalculateCenter());
				}
				else
				{
					CenterViewOn(Vector2.zero);
				}
			}

			GUILayout.FlexibleSpace();

			var label = "";
			if (graphRect.Contains(mousePos))
			{
				label = string.Format("{0}, {1}",
					GraphPosition.x, GraphPosition.y);
			}
			GUILayout.Label(label);

			GUILayout.EndHorizontal();
			GUILayout.EndArea();

			// Draw the graph
			{
				GUI.Box(graphRect, GUIContent.none);

				// Make the clipping window for the graph
				graphRect.position += Vector2.one * GRAPH_PADDING;
				graphRect.size -= Vector2.one * GRAPH_PADDING * 2;
				GUI.BeginClip(graphRect);

				// Draw the graph
				var gridRect = new Rect(Vector2.zero, graphRect.size);
				DrawGrid(gridRect);
				if (Graph != null && Graph.Nodes != null)
				{
					// Make a copy since nodes may reorder the list when drawn
					var nodes = new List<Node>(Graph.Nodes);

					// Draw nodes
					foreach (var node in nodes)
					{
						NodeEditor.DrawNode(this, node);
					}

					// Draw links
					if (Target is Socket)
					{
						var socket = (Socket)Target;
						DrawConnection(rectCache[socket].center,
							Event.current.mousePosition,
							socket.IsInput(Graph), true);
					}
					foreach (var link in Graph.Links)
					{
						if (!rectCache.ContainsKey(link.FromSocket)
							|| !rectCache.ContainsKey(link.ToSocket))
						{
							continue;
						}

						var from = rectCache[link.FromSocket].center;
						var to = rectCache[link.ToSocket].center;
						DrawConnection(from, to, false, false);
					}
				}

				// Search box
				search.OnGUI(this);

				GUI.EndClip();
			}

			switch (Event.current.type)
			{
				case EventType.MouseDown:
					Target = this;
					GUI.FocusControl(null);
					Event.current.Use();
					break;

				case EventType.MouseDrag:
					if (ReferenceEquals(Target, this))
					{
						Offset += Event.current.delta;
						Event.current.Use();
					}
					break;

				case EventType.ContextClick:
					if (graphRect.Contains(Event.current.mousePosition))
					{
						var clipPos = Event.current.mousePosition;
						clipPos.y -= topToolbarRect.size.y + GRAPH_PADDING;
						search.Open(clipPos, GraphPosition);
						Target = search;
						GUI.FocusControl("search_field");
						Event.current.Use();
					}
					break;
			}

			Repaint();
		}

		private Rect GetGraphRect()
		{
			var rect = new Rect(position);
			rect.position = Vector2.up * TOOLBAR_HEIGHT;
			rect.size -= rect.position * 2;
			return rect;
		}

		private void ContextMenuCallback(object obj)
		{
			var arr = (object[])obj;
			var pos = (Vector2)arr[0];
			using (new UndoStack("Add Node To Graph"))
			{
				var node = (Node)CreateInstance((Type)arr[1]);
				node.ID = Graph.NextNodeID++;
				node.XPos = Mathf.FloorToInt(pos.x);
				node.YPos = Mathf.FloorToInt(pos.y);
				Undo.RegisterCreatedObjectUndo(node, null);

				Undo.RecordObject(Graph, null);

				var path = AssetDatabase.GetAssetPath(Graph.GetInstanceID());
				// Check if the graph is a persistent asset or a scene asset
				if (!string.IsNullOrEmpty(path))
				{
					AssetDatabase.AddObjectToAsset(node, Graph);
				}
				Graph.Nodes.Add(node);

				EditorUtility.SetDirty(Graph);
			}
		}

		private GenericMenu GetNodeMenu(Vector2 pos)
		{
			var menu = new GenericMenu();
			var types = GetClassesOfType<Node>();

			foreach (var type in types)
			{
				var attributes = (NodeAttribute[])
					Attribute.GetCustomAttributes(type, typeof(NodeAttribute));
				if (attributes.Length == 0)
				{
					continue;
				}
				var attr = attributes[0];

				menu.AddItem(
					new GUIContent(attr.Path),
					false, ContextMenuCallback,
					new object[] { pos, type });
			}

			return menu;
		}

		private static Type[] GetClassesOfType<T>(params object[] constructorArgs) where T : class
		{
			return (from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
					from assemblyType in domainAssembly.GetTypes()
					where typeof(T).IsAssignableFrom(assemblyType) && !assemblyType.IsAbstract
					select assemblyType).ToArray();
		}

		/// <summary>
		/// Render a node connection between two points.
		/// </summary>
		private static void DrawConnection
			(Vector2 from, Vector2 to, bool left, bool mouse, Color? color = null)
		{
			var size = NodeEditor.SOCKET_RADIUS;
			Handles.DrawBezier(
				new Vector3(from.x + (left ? -size : size), from.y, 0.0f),
				new Vector3(to.x + (mouse ? 0 : (left ? size : -size)), to.y, 0.0f),
				new Vector3(from.x, from.y, 0.0f) + Vector3.right * 50.0f * (left ? -1.0f : 1.0f),
				new Vector3(to.x, to.y, 0.0f) + Vector3.right * 50.0f * (left ? 1.0f : -1.0f),
				color ?? new Color(.4f, .4f, .4f, 1),
				null,
				size / 2
			);
		}

		private void DrawGrid(Rect graphArea)
		{
			var handleColor = Handles.color;

			var offset = Offset;
			offset.x *= -1;

			var coords = new Rect();
			coords.xMin = -Offset.x;
			coords.xMax = -Offset.x + graphArea.size.x;
			coords.yMin = Offset.y - graphArea.size.y;
			coords.yMax = Offset.y;

			var startX = Mathf.RoundToInt(coords.xMin / GRID_CELL_SIZE) * GRID_CELL_SIZE;
			for (; startX < coords.xMax; startX += GRID_CELL_SIZE)
			{
				var v = Calc(coords, new Vector2(startX, coords.center.y), graphArea);
				var from = new Vector2(v.x, v.y + graphArea.size.y / 2);
				var to = new Vector2(v.x, v.y - graphArea.size.y / 2);

				Handles.color = new Color(.5f, .5f, .5f, .5f);
				if (startX == 0)
				{
					Handles.color = new Color(0f, 0f, 0f, 1f);
				}
				else if (startX % (GRID_CELL_SIZE * 10) == 0)
				{
					Handles.color = new Color(.1f, .1f, .1f, .75f);
				}

				Handles.DrawLine(from, to);
			}

			var startY = Mathf.Floor(coords.yMin / GRID_CELL_SIZE) * GRID_CELL_SIZE;
			for (; startY < coords.yMax; startY += GRID_CELL_SIZE)
			{
				var v = Calc(coords, new Vector2(coords.center.x, startY), graphArea);
				var from = new Vector2(v.x + graphArea.size.x / 2, v.y);
				var to = new Vector2(v.x - graphArea.size.x / 2, v.y);

				Handles.color = new Color(.5f, .5f, .5f, .5f);
				if (startY == 0)
				{
					Handles.color = new Color(0f, 0f, 0f, 1f);
				}
				else if (startY % (GRID_CELL_SIZE * 10) == 0)
				{
					Handles.color = new Color(.1f, .1f, .1f, .75f);
				}

				Handles.DrawLine(from, to);
			}

			Handles.color = handleColor;
		}

		private Vector2 Calc(Rect from, Vector2 pos, Rect to)
		{
			var x = (pos.x - from.min.x) / from.width;
			var y = (pos.y - from.min.y) / from.height;

			x = to.min.x + (to.width * x);
			y = to.min.y + (to.height * (1 - y));
			return new Vector2(x, y);
		}

		public void CenterViewOn(Vector2 newPosition)
		{
			var pos = new Vector2(-newPosition.x, newPosition.y);
			Offset = pos + GetGraphRect().size / 2;
		}
	}
}