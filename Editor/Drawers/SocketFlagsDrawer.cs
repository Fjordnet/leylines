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
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// The Property Drawer for the SocketFlags enum.
	/// </summary>
	[CustomPropertyDrawer(typeof(SocketFlags))]
	public class SocketFlagsDrawer : PropertyDrawer
	{
		public override void OnGUI
			(Rect rect, SerializedProperty prop, GUIContent label)
		{
			var targetEnum = GetBaseProperty<SocketFlags>(prop);
			var type = targetEnum.GetType();

			EditorGUI.BeginProperty(rect, label, prop);
			Enum enumNew = EditorGUI.EnumMaskField(rect, label, targetEnum);
			prop.intValue = (int)Convert.ChangeType(enumNew, type);
			EditorGUI.EndProperty();
		}

		private static T GetBaseProperty<T>(SerializedProperty prop)
		{
			string[] separatedPaths = prop.propertyPath.Split('.');

			object target = prop.serializedObject.targetObject;
			foreach (var item in separatedPaths)
			{
				if (item == "Array")
				{
					continue;
				}
				else if (item.StartsWith("data["))
				{
					var indexStr = item.Substring(5, item.Length - 6);
					var index = int.Parse(indexStr);

					var array = (IEnumerable)target;
					target = array.Cast<object>().ElementAt(index);
				}
				else
				{
					var flags = BindingFlags.Instance
							| BindingFlags.Public | BindingFlags.NonPublic;

					var fieldInfo = target.GetType().GetField(item, flags);
					target = fieldInfo.GetValue(target);
				}
			}
			return (T)target;
		}
	}
}
