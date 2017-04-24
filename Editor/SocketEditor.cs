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
			var graph = editor.Graph;
			GUI.Box(rect, GUIContent.none);

			editor.rectCache[socket] = rect;

			switch (Event.current.type)
			{
				case EventType.MouseDown:
					if (rect.Contains(Event.current.mousePosition))
					{
						editor.Target = socket;
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
