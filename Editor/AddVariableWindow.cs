using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

namespace Exodrifter.NodeGraph
{
	public class AddVariableWindow : EditorWindow
	{
		#region Launchers

		private static AddVariableWindow instance;

		public static void Launch(VariablesEditor editor, ReorderableList list)
		{
			if (instance == null)
			{
				instance = CreateInstance<AddVariableWindow>();
			}
			instance.titleContent = new GUIContent("Add Variable");
			instance.list = list;
			instance.editor = editor;
			instance.ShowUtility();
			instance.Focus();

			var size = new Vector2(200, 110);
			instance.minSize = size;
			instance.maxSize = size;

			var rect = instance.position;
			rect.size = size;
			rect.center = GUIUtility.GUIToScreenPoint(XEvent.MousePos);
			instance.position = rect;
		}

		#endregion

		private VariablesEditor editor;
		private ReorderableList list;

		private string varName;
		private string typeStr;

		void OnGUI()
		{
			if (list == null || list.Equals(null))
			{
				Close();
				return;
			}

			XGUI.ResetToStyle(null);
			XGUI.BeginVertical();

			XGUI.ResetToStyle(GUI.skin.textField);

			XGUI.LabelWidth = 40;
			varName = XGUI.TextField("Name", varName);
			typeStr = XGUI.TextField("Type", typeStr);

			XGUI.ResetToStyle(null);
			XGUI.BeginHorizontal();

			XGUI.ResetToStyle(GUI.skin.button);
			if (XGUI.Button("Cancel"))
			{
				Close();
				return;
			}

			XGUI.Enabled = IsValid();
			if (XGUI.Button("Create"))
			{
				var value = new DynamicValue();
				value.TypeString = typeStr;
				list.list.Add(new Variable(varName, value));
				editor.SaveList();
				Close();
				return;
			}

			XGUI.EndHorizontal();

			if (!IsNameValid())
			{
				EditorGUILayout.HelpBox(
					"Variable name is empty!",
					MessageType.Warning);
			}
			else if (!IsNameUnique())
			{
				EditorGUILayout.HelpBox(
					"Variable name is the same as an existing variable!",
					MessageType.Warning);
			}
			else if (!IsTypeValid())
			{
				EditorGUILayout.HelpBox(
					"Variable type does not exist!",
					MessageType.Warning);
			}

			XGUI.EndVertical();
		}

		bool IsValid()
		{
			return IsNameValid() && IsNameUnique() && IsTypeValid();
		}

		bool IsNameValid()
		{
			return !string.IsNullOrEmpty(varName);
		}

		bool IsNameUnique()
		{
			if (list == null)
			{
				return false;
			}

			foreach (var item in list.list)
			{
				if (((Variable)item).Name == varName)
				{
					return false;
				}
			}

			return true;
		}

		bool IsTypeValid()
		{
			if (string.IsNullOrEmpty(typeStr))
			{
				return false;
			}

			return Type.GetType(typeStr) != null;
		}
	}
}