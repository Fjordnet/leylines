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
		public static GraphEditor Instance { get; private set; }

		public static bool IsOpen
		{
			get { return Instance != null; }
		}

		[SerializeField]
		private bool graphIDSet = false;
		[SerializeField]
		private int graphID = 0;

		private static NodeGraphSkin skin;
		public static NodeGraphSkin Skin
		{
			get
			{
				if (skin != null)
				{
					return skin;
				}

				string[] guids = AssetDatabase.FindAssets("t:NodeGraphSkin");
				foreach (var guid in guids)
				{
					var path = AssetDatabase.GUIDToAssetPath(guid);
					skin = AssetDatabase.LoadAssetAtPath<NodeGraphSkin>(path);
				}
				return skin;
			}
		}

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

		public object Target { get; set; }
		public Tooltip Tooltip { get; set; }

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
			window.CenterView();
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

		public static Texture2D flatTexture;
		public static Texture2D boxTexture;

		void Awake()
		{
			Instance = this;
		}

		void OnDestroy()
		{
			Instance = null;
		}

		void OnGUI()
		{
			Tooltip = null;

			// Make the box texture opaque
			if (EditorGUIUtility.isProSkin && boxTexture == null)
			{
				// Make a copy of the old texture
				var oldTex = GUI.skin.box.normal.background;
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
			if (flatTexture == null)
			{
				flatTexture = new Texture2D(1, 1);
				flatTexture.filterMode = FilterMode.Point;
				flatTexture.SetPixels(new Color[1] { Color.white });
				flatTexture.Apply();
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
			topToolbarRect.position = Vector2.zero;
			topToolbarRect.size -= new Vector2(10, 0);
			topToolbarRect.height = toolbarHeight.y;

			XGUI.ResetToStyle(null);
			XGUI.BeginArea(new Rect(0, 0, position.width - 10, TOOLBAR_HEIGHT));
			XGUI.BeginHorizontal();

			var oldGraph = Graph;
			XGUI.ResetToStyle(null);
			XGUI.BeginVertical();
			XGUI.FlexibleSpace();
			Graph = XGUI.ObjectField(Graph, false);
			if (Graph != oldGraph)
			{
				CenterView();
			}
			XGUI.FlexibleSpace();
			XGUI.EndVertical();

			XGUI.FlexibleSpace();

			XGUI.ResetToStyle(GUI.skin.button);
			if (XGUI.Button("Variables"))
			{
				VariablesEditor.Launch(this);
			}

			XGUI.EndHorizontal();
			XGUI.EndArea();

			// Bottom Toolbar
			XGUI.ResetToStyle(null);
			XGUI.BeginArea(new Rect(0, position.size.y - TOOLBAR_HEIGHT, position.width - 10, TOOLBAR_HEIGHT));
			XGUI.BeginHorizontal();

			XGUI.ResetToStyle(GUI.skin.button);
			XGUI.Enabled = Graph != null;
			snap = XGUI.ToggleButton(snap, "Snap");

			XGUI.ResetToStyle(GUI.skin.button);
			XGUI.Enabled = Graph != null;
			if (XGUI.Button("Center View"))
			{
				CenterView();
			}

			XGUI.FlexibleSpace();

			if (graphRect.Contains(mousePos))
			{
				XGUI.ResetToStyle(GUI.skin.label);
				XGUI.Alignment = TextAnchor.MiddleRight;
				XGUI.Label(
					string.Format("{0}, {1}", GraphPosition.x, GraphPosition.y),
					XGUI.ExpandHeight(true));
			}

			XGUI.EndHorizontal();
			XGUI.EndArea();

			// Draw the graph
			{
				XGUI.ResetToStyle(null);
				XGUI.BackgroundColor = Skin.canvasColor;
				XGUI.Normal.background = flatTexture;
				XGUI.Box(graphRect);

				// Make the clipping window for the graph
				graphRect.position += Vector2.one * GRAPH_PADDING;
				graphRect.size -= Vector2.one * GRAPH_PADDING * 2;
				XGUI.ResetToStyle(null);
				XGUI.BeginClip(graphRect);

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

						if (rectCache.ContainsKey(socket))
						{
							DrawConnection(rectCache[socket].center,
								Event.current.mousePosition,
								socket.IsInput(Graph), true,
								Skin.tempLinkColor);
						}
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

						var socketType = link.FromSocket.GetSocketType(Graph);
						var color = Skin.objectSocketColor;
						if (socketType == typeof(ExecType))
						{
							color = Skin.execSocketColor;
						}
						if (socketType.IsPrimitive)
						{
							color = Skin.primitiveSocketColor;
						}

						if (search.IsOpen)
						{
							color = Skin.TintColor(color, Skin.disabledSocketTint);
						}
						DrawConnection(from, to, false, false, color);
					}
				}

				// Draw Tooltip
				if (Tooltip != null)
				{
					Tooltip.OnGUI();
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
						node.DisplayName = obj.GetType().Name;
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

			Repaint();

			if (GUI.changed && Graph != null && !Graph.Equals(null))
			{
				EditorUtility.SetDirty(Graph);
			}
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
			(Vector2 from, Vector2 to, bool left, bool mouse, Color color)
		{
			var size = NodeEditor.SOCKET_RADIUS;
			Handles.DrawBezier(
				new Vector3(from.x + (left ? -size : size), from.y, 0.0f),
				new Vector3(to.x + (mouse ? 0 : (left ? size : -size)), to.y, 0.0f),
				new Vector3(from.x, from.y, 0.0f) + Vector3.right * 50.0f * (left ? -1.0f : 1.0f),
				new Vector3(to.x, to.y, 0.0f) + Vector3.right * 50.0f * (left ? 1.0f : -1.0f),
				color,
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

				Handles.color = Skin.canvasCellLineColor;
				if (startX == 0)
				{
					Handles.color = Skin.canvasAxisLineColor;
				}
				else if (startX % (GRID_CELL_SIZE * 10) == 0)
				{
					Handles.color = Skin.canvasChunkLineColor;
				}

				Handles.DrawLine(from, to);
			}

			var startY = Mathf.Floor(coords.yMin / GRID_CELL_SIZE) * GRID_CELL_SIZE;
			for (; startY < coords.yMax; startY += GRID_CELL_SIZE)
			{
				var v = Calc(coords, new Vector2(coords.center.x, startY), graphArea);
				var from = new Vector2(v.x + graphArea.size.x / 2, v.y);
				var to = new Vector2(v.x - graphArea.size.x / 2, v.y);

				Handles.color = Skin.canvasCellLineColor;
				if (startY == 0)
				{
					Handles.color = Skin.canvasAxisLineColor;
				}
				else if (startY % (GRID_CELL_SIZE * 10) == 0)
				{
					Handles.color = Skin.canvasChunkLineColor;
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

		private void CenterView()
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

		private void CenterViewOn(Vector2 newPosition)
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