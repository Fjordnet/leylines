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
using Exodrifter.NodeGraph;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Exodrifter
{
	public static class NodeEditor
	{
		internal const int LINE_PADDING = 3;
		internal const int BOX_PADDING = 5;
		internal const int SOCKET_RADIUS = 6;
		internal const int SOCKET_PADDING = 4;
		internal const int MIDDLE_PADDING = 8;

		public static void DrawNode(GraphEditor editor, Node node)
		{
			EditorGUI.BeginChangeCheck();

			if (Util.IsNull(node))
			{
				return;
			}

			var serializedObject = new SerializedObject(node);

			var inputs = node.GetInputSockets();
			var outputs = node.GetOutputSockets();

			var inputWidth = GetContentWidth(node, inputs);
			var inputHeight = GetPropertyHeights(serializedObject, node, inputs, editor);
			var outputWidth = GetContentWidth(node, outputs);
			var outputHeight = GetPropertyHeights(serializedObject, node, outputs, editor);
			var height = Mathf.Max(inputHeight, outputHeight)
				+ LINE_PADDING + EditorGUIUtility.singleLineHeight;

			// Draw box
			var rect = new Rect();
			rect.width = inputWidth + outputWidth + BOX_PADDING * 2
				+ SOCKET_RADIUS * 4 + SOCKET_PADDING * 2 + MIDDLE_PADDING;
			if (inputWidth == 0 || outputWidth == 0)
			{
				rect.width -= MIDDLE_PADDING + SOCKET_RADIUS * 2
					+ SOCKET_PADDING;
			}
			rect.height = height + BOX_PADDING * 2;
			rect.center = new Vector2(node.XPos, -node.YPos) + editor.Offset;

			var fullRect = new Rect(rect);
			GUI.Box(rect, GUIContent.none);

			var labelRect = new Rect(rect);
			labelRect.x += BOX_PADDING;
			labelRect.y += BOX_PADDING;
			labelRect.height = EditorGUIUtility.singleLineHeight;
			GUI.Label(labelRect, node.DisplayName);

			rect.x += BOX_PADDING;
			if (inputWidth != 0)
			{
				rect.x += SOCKET_RADIUS * 2 + SOCKET_PADDING;
			}
			rect.y = labelRect.y + labelRect.height + LINE_PADDING;
			rect.width = inputWidth;
			var leftRect = new Rect(rect);

			if (inputWidth != 0)
			{
				rect.x += inputWidth + MIDDLE_PADDING;
			}
			rect.width = outputWidth;
			var rightRect = new Rect(rect);

			DrawMembers(true, editor, node, leftRect, serializedObject, inputs);
			DrawMembers(false, editor, node, rightRect, serializedObject, outputs);

			rect = fullRect;
			switch (Event.current.type)
			{
				case EventType.MouseDown:
					if (rect.Contains(Event.current.mousePosition))
					{
						editor.Target = node;
						Event.current.Use();
					}
					break;

				case EventType.MouseDrag:
					if (ReferenceEquals(editor.Target, node))
					{
						Undo.RecordObject(node, "Move " + node.GetType().Name);

						if (editor.Snap <= 0)
						{
							node.XPos += (int)Event.current.delta.x;
							node.YPos -= (int)Event.current.delta.y;
						}
						else
						{
							var graphPos = editor.GraphPosition;
							node.XPos = Mathf.RoundToInt(graphPos.x / editor.Snap) * editor.Snap;
							node.YPos = Mathf.RoundToInt(graphPos.y / editor.Snap) * editor.Snap;
						}

						Event.current.Use();
					}
					break;

				case EventType.MouseUp:
					if (ReferenceEquals(editor.Target, node))
					{
						editor.Target = null;
						Event.current.Use();
					}
					break;

				case EventType.KeyDown:
					switch(Event.current.keyCode)
					{
						case KeyCode.Delete:
							if (!ReferenceEquals(editor.Target, node))
							{
								break;
							}

							using (new UndoStack("Remove " + node.DisplayName + " Node"))
							{
								Undo.RegisterCompleteObjectUndo(editor.Graph, null);
								Undo.DestroyObjectImmediate(node);
							}
							editor.Graph.Links.RemoveAllWith(node);
							editor.Graph.Nodes.Remove(node);
							editor.Target = null;
							Event.current.Use();
							break;
					}
					break;
			}

			if (EditorGUI.EndChangeCheck())
			{
				serializedObject.ApplyModifiedProperties();
			}
		}

		private static void DrawMembers
			(bool isInput, GraphEditor editor, Node node, Rect rect,
			SerializedObject serializedObject, IEnumerable<Socket> sockets)
		{
			rect = new Rect(rect);

			foreach (var socket in sockets)
			{
				// Draw the socket
				var h = EditorGUIUtility.singleLineHeight;
				var socketRect = new Rect(rect);
				if (isInput)
				{
					socketRect.x -= SOCKET_RADIUS * 2 + SOCKET_PADDING;
				}
				else
				{
					socketRect.x += socketRect.width + SOCKET_PADDING;
				}
				socketRect.width = SOCKET_RADIUS * 2;
				socketRect.height = SOCKET_RADIUS * 2;
				socketRect.y += (h - socketRect.height) / 2;
				SocketEditor.DrawSocket(editor, node, socket, socketRect);

				// Draw the member
				DrawMember(isInput, editor, node, ref rect, serializedObject, socket);
				rect.y += rect.height + LINE_PADDING;
			}
		}

		private static void DrawMember
			(bool isInput, GraphEditor editor, Node node, ref Rect rect,
			SerializedObject serializedObject, Socket socket)
		{
			string name = node.GetSocketDisplayName(socket);
			bool editable = node.GetSocketFlags(socket).IsEditable();
			bool linked = editor.Graph.Links.IsSocketLinkedTo(socket);

			if (editable && !linked)
			{
				rect.height = EditorGUIUtility.singleLineHeight;
				var type = node.GetSocketType(socket);
				var value = node.GetSocketValue(socket);

				var prop = serializedObject.FindProperty(socket.FieldName);
				if (prop != null)
				{
					EditorGUI.PropertyField(rect, prop, GUIContent.none, true);
				}
				else if (type == null)
				{
					GUI.enabled = false;
					EditorGUI.TextField(rect, GUIContent.none, "Unknown Type");
					GUI.enabled = true;
				}
				else
				{
					value = GUIExtension.DrawField
						(rect, value, type, GUIContent.none, true);
					node.SetSocketValue(socket, value);
				}
			}
			else
			{
				rect.height = EditorGUIUtility.singleLineHeight;
				EditorGUI.LabelField(rect, name);
			}

			// Prepare a tooltip
			if (rect.Contains(Event.current.mousePosition) && editor.Target == null)
			{
				var text = string.Format(
					"<color=#4aa><i>{0}</i></color> <b>{1}</b>\n{2}",
					node.GetSocketType(socket).Name,
					node.GetSocketDisplayName(socket),
					string.IsNullOrEmpty(node.GetSocketDescription(socket))
						? "<color=#777><i>No documentation</i></color>"
						: node.GetSocketDescription(socket)
				);
				editor.Target = new Tooltip(text);
			}
		}

		private static float GetPropertyHeights
			(SerializedObject serializedObject, Node node,
			IEnumerable<Socket> sockets, GraphEditor editor)
		{

			float height = 0f;
			foreach (var socket in sockets)
			{
				var editable = node.GetSocketFlags(socket).IsEditable();
				var linked = editor.Graph.Links.IsSocketLinkedTo(socket);

				if (editable && !linked)
				{
					var type = node.GetSocketType(socket);
					var value = node.GetSocketValue(socket);

					var prop = serializedObject.FindProperty(socket.FieldName);
					if (prop != null)
					{
						height += EditorGUI.GetPropertyHeight(prop, GUIContent.none);
					}
					else if (type == null)
					{
						height += EditorGUIUtility.singleLineHeight;
					}
					else
					{
						height += GUIExtension.GetFieldHeight(value, type, GUIContent.none);
					}

					height += LINE_PADDING;
				}
				else
				{
					height += EditorGUIUtility.singleLineHeight;
					height += LINE_PADDING;
				}
			}

			return height - LINE_PADDING;
		}

		private static float GetContentWidth
			(Node node, IEnumerable<Socket> sockets)
		{
			float? max = null;
			foreach (var socket in sockets)
			{
				var type = node.GetSocketType(socket);
				if (type == null)
				{
					var name = "Unknown Type";

					float width = GUI.skin.textField.CalcSize
						(new GUIContent(name)).x;
					max = max ?? width;
					max = Mathf.Max(max.Value, width);
				}
				else if (node.GetSocketFlags(socket).IsEditable())
				{
					float width = node.GetSocketWidth(socket);
					max = max ?? width;
					max = Mathf.Max(max.Value, width);
				}
				else
				{
					var name = node.GetSocketDisplayName(socket);

					float width = GUI.skin.label.CalcSize
						(new GUIContent(name)).x;
					max = max ?? width;
					max = Mathf.Max(max.Value, width);
				}
			}
			return max ?? 0;
		}

		private static bool IsOverride(MethodInfo m)
		{
			return m.GetBaseDefinition().DeclaringType != m.DeclaringType;
		}
	}
}
