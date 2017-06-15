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
	public static class SocketEditor
	{
		public static void DrawSocket
			(GraphEditor editor, Node node, Socket socket, Rect rect)
		{
			editor.rectCache[socket] = rect;

			var socketType = socket.GetSocketType(editor.Graph);
			var color = GraphEditor.Skin.objectSocketColor;
			var style = GraphEditor.Skin.paramSocketStyle;
			if (socketType == typeof(ExecType))
			{
				color = GraphEditor.Skin.execSocketColor;
				style = GraphEditor.Skin.execSocketStyle;
			}
			else if (socketType.IsPrimitive)
			{
				color = GraphEditor.Skin.primitiveSocketColor;
			}

			var on = editor.Graph.Links.IsSocketLinkedTo(socket)
				|| editor.Graph.Links.IsSocketLinkedFrom(socket);

			XGUI.ResetToStyle(null);
			XGUI.Normal.background = on ? style.onSocketTexture : style.offSocketTexture;
			XGUI.Color = on ? color : GraphEditor.Skin.TintColor(color, GraphEditor.Skin.offSocketTint);
			XGUI.Color = !editor.search.IsOpen ? XGUI.Color : GraphEditor.Skin.TintColor(XGUI.Color, GraphEditor.Skin.disabledSocketTint);
			XGUI.Box(rect);

			if (!GUI.enabled)
			{
				return;
			}

			switch (Event.current.type)
			{
				case EventType.MouseDown:
					if (rect.Contains(Event.current.mousePosition))
					{
						if (Event.current.control)
						{
							editor.Graph.Links.RemoveAllWith(socket);
						}
						else
						{
							editor.Target = socket;
						}
						Event.current.Use();
					}
					break;

				case EventType.MouseUp:
					if (!(editor.Target is Socket))
					{
						break;
					}
					if (!rect.Contains(Event.current.mousePosition))
					{
						break;
					}

					var otherSocket = (Socket)editor.Target;
					if (socket.Equals(otherSocket))
					{
						editor.Target = null;
						break;
					}
					if (socket.NodeID == otherSocket.NodeID)
					{
						editor.Target = null;
						break;
					}

					var graph = editor.Graph;

					if (socket.IsInput(graph) == otherSocket.IsInput(graph))
					{
						editor.Target = null;
						break;
					}
					if (socket.GetSocketType(graph) != otherSocket.GetSocketType(graph))
					{
						if (!socket.GetSocketType(graph)
							.IsAssignableFrom(otherSocket.GetSocketType(graph)))
						{
							editor.Target = null;
							break;
						}
					}

					// Record to graph
					Undo.RegisterCompleteObjectUndo(graph,
						string.Format("Link {0} and {1}",
							socket,
							otherSocket
						)
					);
					if (!socket.GetFlags(graph).AllowMultipleLinks())
					{
						graph.Links.RemoveAllWith(socket);
					}
					if (!otherSocket.GetFlags(graph).AllowMultipleLinks())
					{
						graph.Links.RemoveAllWith(otherSocket);
					}
					graph.Links.Add(graph, socket, otherSocket);

					editor.Target = null;

					GUI.changed = true;
					Event.current.Use();
					break;
			}
		}
	}
}
