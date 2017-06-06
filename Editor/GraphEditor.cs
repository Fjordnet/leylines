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
using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[Serializable]
	public class GraphEditor : EditorWindow
	{
		[SerializeField]
		private bool graphIDSet = false;
		[SerializeField]
		private int graphID = 0;

		#region Properties

		private Graph graph;
		public Graph Graph
		{
			get
			{
				if (Util.IsNull(graph) && graphIDSet)
				{
					var obj = EditorUtility.InstanceIDToObject(graphID);
					if (obj is Graph)
					{
						graph = (Graph)obj;
					}
					else
					{
						graphIDSet = false;
					}
				}

				return graph;
			}
			set
			{
				graph = value;

				if (!Util.IsNull(graph))
				{
					graphID = graph.GetInstanceID();
					graphIDSet = true;
				}
			}
		}

		private Dictionary<Type, object> clipboard;
		public Dictionary<Type, object> Clipboard
		{
			get
			{
				clipboard = clipboard ?? new Dictionary<Type, object>();
				return clipboard;
			}
		}

		private object target;
		public object Target
		{
			get { return target; }
			set { PreviousTarget = target; target = value; }
		}

		public object PreviousTarget { get; set; }
		public Vector2 Offset { get; set; }
		public int Snap { get { return snap ? GRID_CELL_SIZE : 0; } }
		public Vector2 GraphPosition { get; private set; }

		#endregion

		public const int GRAPH_PADDING = 2;
		public const int GRID_CELL_SIZE = 20;
		public const int TOOLBAR_HEIGHT = 23;

		private bool snap = false;
		public SearchEditor search = new SearchEditor(new Vector2(500, 300));

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

		[SerializeField]
		private Texture2D oldTex;
		[SerializeField]
		private Texture2D boxTexture;

		void OnGUI()
		{
			// Looks like the editor crashed before setting back the old
			// texture at the end of the method
			var oldTex = GUI.skin.box.normal.background;
			if (oldTex == null)
			{
				oldTex = this.oldTex;
				GUI.skin.box.normal.background = oldTex;
			}
			this.oldTex = oldTex;

			// Make the box texture opaque
			if (EditorGUIUtility.isProSkin)
			{
				if (boxTexture == null && oldTex != null)
				{
					// Make a copy of the old texture
					var tmp = RenderTexture.GetTemporary(oldTex.width, oldTex.height);

					Graphics.Blit(oldTex, tmp);
					RenderTexture previous = RenderTexture.active;
					RenderTexture.active = tmp;
					boxTexture = new Texture2D(oldTex.width, oldTex.height);
					boxTexture.ReadPixels(new Rect(0, 0, tmp.width, tmp.height), 0, 0);
					RenderTexture.active = previous;
					RenderTexture.ReleaseTemporary(tmp);

					// Remove alpha
					var colors = boxTexture.GetPixels();
					for (int i = 0; i < colors.Length; ++i)
					{
						// Pro background color is RGB(64, 64, 64)
						colors[i].r = 0.2196f + (colors[i].r * colors[i].a);
						colors[i].g = 0.2196f + (colors[i].g * colors[i].a);
						colors[i].b = 0.2196f + (colors[i].b * colors[i].a);
						colors[i].a = 1;
					}

					boxTexture.SetPixels(colors);
					boxTexture.Apply();
				}

				GUI.skin.box.normal.background = boxTexture;
			}

			rectCache = rectCache ?? new Dictionary<Socket, Rect>();
			rectCache.Clear();

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

			GUI.enabled = Graph != null;
			if (GUILayout.Button("Save", GUILayout.MaxHeight(15)))
			{
				AssetDatabase.SaveAssets();
			}
			GUI.enabled = true;

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

			GUI.enabled = Graph != null;
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
			GUI.enabled = true;

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

				// Draw Tooltip
				if (Target is Tooltip)
				{
					(Target as Tooltip).OnGUI();
					Target = null;
				}

				// Search box
				search.OnGUI(this);

				GUI.EndClip();
			}

			if (Graph == null)
			{
				CenterViewOn(Vector2.zero);

				var menuRect = new Rect();
				menuRect.size = new Vector2(300, 200);
				menuRect.center = graphRect.center;
				GUI.Box(menuRect, GUIContent.none);
				GUILayout.BeginArea(menuRect);
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal(GUILayout.Height(150));
				GUILayout.FlexibleSpace();
				GUILayout.BeginVertical(GUILayout.Width(250));

				GUILayout.FlexibleSpace();

				GUILayout.BeginHorizontal();
				var size = GUI.skin.label.fontSize;
				var alignment = GUI.skin.label.alignment;
				GUI.skin.label.fontSize = 20;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("Leylines");
				GUI.skin.label.fontSize = size;
				GUI.skin.label.alignment = alignment;
				GUILayout.EndHorizontal();

				GUILayout.FlexibleSpace();

				GUILayout.BeginVertical(GUILayout.Height(130));
				var rich = GUI.skin.label.richText;
				GUI.skin.label.richText = true;
				GUI.skin.label.alignment = TextAnchor.MiddleCenter;
				GUILayout.Label("<b>No Graph Loaded</b>");
				GUI.skin.label.richText = rich;
				GUI.skin.label.alignment = alignment;
				GUILayout.EndVertical();

				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.EndArea();
			}

			switch (Event.current.type)
			{
				case EventType.MouseDown:
					Target = this;
					GUI.FocusControl(null);
					Event.current.Use();
					break;

				case EventType.MouseDrag:
					if (Graph == null)
					{
						break;
					}
					if (ReferenceEquals(Target, this))
					{
						Offset += Event.current.delta;
						Event.current.Use();
					}
					break;

				case EventType.MouseUp:
					if (ReferenceEquals(Target, this))
					{
						Target = null;
						GUI.FocusControl(null);
					}
					if (Target != null && Target.GetType() == typeof(Socket))
					{
						var clipPos = Event.current.mousePosition;
						clipPos.y -= topToolbarRect.size.y + GRAPH_PADDING;
						var socket = (Socket)Target;
						if (socket.IsInput(Graph))
						{
							search.SetWantedOutputContext(this, socket);
						}
						else
						{
							search.SetWantedInputContext(this, socket);
						}
						search.Open(clipPos, GraphPosition, Graph.Policy, true);
						Target = search;
						GUI.FocusControl("search_field");
						Event.current.Use();
					}
					if (Event.current.button == 1 && Graph != null)
					{
						if (graphRect.Contains(Event.current.mousePosition))
						{
							var clipPos = Event.current.mousePosition;
							clipPos.y -= topToolbarRect.size.y + GRAPH_PADDING;
							search.UnsetContext();
							search.Open(clipPos, GraphPosition, Graph.Policy);
							Target = search;
							GUI.FocusControl("search_field");
							Event.current.Use();
						}
					}
					break;

				case EventType.DragUpdated:
					if (DragAndDrop.objectReferences.Length == 1)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
					}
					else
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
					}
					Event.current.Use();
					break;

				case EventType.DragPerform:
					if (DragAndDrop.objectReferences.Length == 1)
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
						DragAndDrop.AcceptDrag();

						var obj = DragAndDrop.objectReferences[0];
						var type = obj.GetType();

						var node = CreateInstance<DynamicNode>();
						var socket = new DynamicSocket(type, type.Name,
							SocketFlags.AllowMultipleLinks | SocketFlags.Editable);
						socket.SocketValue = obj;
						node.AddOutputSocket(socket);
						AddNode(node, GraphPosition);
					}
					else
					{
						DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
					}
					Event.current.Use();
					break;
			}

			GUI.skin.box.normal.background = oldTex;
			Repaint();
		}

		private Rect GetGraphRect()
		{
			var rect = new Rect(position);
			rect.position = Vector2.up * TOOLBAR_HEIGHT;
			rect.size -= rect.position * 2;
			return rect;
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

		public void AddNode(Node node, Vector2 pos)
		{
			using (new UndoStack("Add Node To Graph"))
			{
				node.ID = Graph.NextNodeID++;
				node.Pos = pos;
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
	}
}