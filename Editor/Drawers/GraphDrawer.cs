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
	/// The Property Drawer for Graph types.
	/// </summary>
	[CustomPropertyDrawer(typeof(Graph))]
	public class GraphDrawer : PropertyDrawer
	{
		public override void OnGUI
			(Rect rect, SerializedProperty prop, GUIContent label)
		{
			// Show Object field
			var r = new Rect(rect);
			r.width -= 40;
			var obj = prop.objectReferenceValue;
			obj = EditorGUI.ObjectField(r, label, obj, typeof(Graph), false);
			prop.objectReferenceValue = obj;

			// Show New/Edit button
			var buttonRect = new Rect(rect);
			buttonRect.x += r.width;
			buttonRect.width = 40;
			if (Util.IsNull(obj))
			{
				if (GUI.Button(buttonRect, "New"))
				{
					obj = ScriptableObject.CreateInstance<Graph>();
					prop.objectReferenceValue = obj;
				}
			}
			else
			{
				if (GUI.Button(buttonRect, "Edit"))
				{
					GraphEditor.Launch((Graph)obj);
				}
			}
		}
	}
}
