using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Exodrifter.NodeGraph
{
	public class VariablesEditor : EditorWindow
	{
		private GraphEditor editor;

		#region Launchers

		public static void Launch(GraphEditor editor)
		{
			var dockTo = Type.GetType("UnityEditor.SceneHierarchyWindow,UnityEditor.dll");
			var window = GetWindow<VariablesEditor>(dockTo);

			window.titleContent = new GUIContent("Variables");
			window.editor = editor;
			window.Show();
			window.Focus();
		}

		#endregion

		public ReorderableList list;

		void OnGUI()
		{
			XGUI.ResetToStyle(null);
			XGUI.BeginVertical();

			if (editor == null || editor.Equals(null))
			{
				editor = GraphEditor.Instance;
			}

			// Check if the graph editor exists
			if (editor == null || editor.Equals(null)
				|| editor.Graph == null || editor.Graph.Equals(null))
			{
				XGUI.ResetToStyle(GUI.skin.label);
				XGUI.Padding = new RectOffset(0, 0, 10, 0);
				XGUI.Alignment = TextAnchor.MiddleCenter;
				XGUI.FontStyle = FontStyle.Italic;
				XGUI.Label("No open graph");

				XGUI.FlexibleSpace();
			}
			else
			{
				InitList();

				list.DoLayoutList();
			}

			XGUI.EndVertical();
		}

		public void SaveList()
		{
			editor.Graph.Variables = new VariableDictionary(
				list.list.Cast<Variable>().ToList());
			GUI.changed = true;
		}

		private void InitList()
		{
			if (list == null)
			{
				list = new ReorderableList
					(editor.Graph.Variables.AsList(), typeof(DynamicValue));
				list.headerHeight = 0;
				list.elementHeightCallback += ElementHeight;
				list.drawElementCallback += DrawElement;
				list.onAddCallback += OnAdd;
				list.onRemoveCallback = OnRemoveCallback;
			}
		}

		void OnAdd(ReorderableList list)
		{
			AddVariableWindow.Launch(this, list);
		}

		void OnRemoveCallback(ReorderableList list)
		{
			if (EditorUtility.DisplayDialog
				("Warning!", "Are you sure?", "Yes", "No")) {
				ReorderableList.defaultBehaviours.DoRemoveButton(list);
			}
		}

		private float ElementHeight(int index)
		{
			var height = EditorGUIUtility.singleLineHeight
				+ EditorGUIUtility.standardVerticalSpacing * 4;

			var item = (Variable)list.list[index];
			if (item == null || item.Value == null || item.Value.Value == null)
			{
				height += EditorGUIUtility.singleLineHeight;
			}
			else
			{
				var value = item.Value.Value;
				height += GUIExtension.GetFieldHeight
					(value, value.GetType(), XGUI.None);
			}

			return height;
		}

		private void DrawElement
			(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = (Variable)list.list[index];

			var width = rect.width;
			var height = rect.height;

			rect.y += EditorGUIUtility.standardVerticalSpacing;
			rect.height = EditorGUIUtility.singleLineHeight;
			XGUI.ResetToStyle(GUI.skin.label);
			XGUI.RichText = true;
			var str = item.Value == null ? null : item.Value.TypeString;
			XGUI.Label(rect, string.Format("<b>{0}</b> <i>{1}</i>", item.Name, str));

			rect.y += rect.height + EditorGUIUtility.standardVerticalSpacing;
			rect.height = height - EditorGUIUtility.singleLineHeight
				- EditorGUIUtility.standardVerticalSpacing * 4;

			EditorGUI.BeginChangeCheck();

			var type = item.Value == null
				? null : Type.GetType(item.Value.TypeString);
			if (type != null)
			{
				if (typeof(UnityEngine.Object).IsAssignableFrom(type))
				{
					item.Value.Value = EditorGUI.ObjectField(rect, (UnityEngine.Object)item.Value.Value, type, true);
				}
				else
				{
					var oldValue = item.Value.Value;
					var newValue = GUIExtension.DrawField(rect, oldValue, type, XGUI.None, true);
					item.Value.Value = newValue;
				}
			}
			else
			{
				XGUI.ResetToStyle(GUI.skin.label);
				XGUI.Enabled = false;
				XGUI.TextField(rect, "Unknown Type");
			}

			if (EditorGUI.EndChangeCheck())
			{
				EditorUtility.SetDirty(editor.Graph);
			}
		}
	}
}