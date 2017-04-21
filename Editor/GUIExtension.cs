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
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	public static class GUIExtension
	{
		private static Dictionary<Type, Type> emitTypeCache;
		private static Dictionary<Type, ScriptableObject> emitScriptableObjectCache;

		public static object DrawField
			(Rect rect, object value, Type t, GUIContent label, bool includeChildren)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t", "t cannot be null!");
			}

			// Create a mock type
			var type = EmitType(t);
			var obj = EmitScriptableObject(t, type);
			if (value != null && t.IsAssignableFrom(value.GetType()))
			{
				type.GetField("value").SetValue(obj, value);
			}

			// Draw the field
			var so = new SerializedObject(obj);
			var prop = so.FindProperty("value");
			EditorGUI.PropertyField(rect, prop, label, includeChildren);

			// Get the new value
			prop.serializedObject.ApplyModifiedProperties();
			var ret = obj.GetType().GetField("value").GetValue(obj);
			UnityEngine.Object.DestroyImmediate(obj);

			return ret;
		}

		public static float GetFieldHeight
			(object value, Type t, GUIContent label)
		{
			if (t == null)
			{
				throw new ArgumentNullException("t", "t cannot be null!");
			}

			// Create a mock type
			var type = EmitType(t);
			var obj = EmitScriptableObject(t, type);
			if (value != null && t.IsAssignableFrom(value.GetType()))
			{
				type.GetField("value").SetValue(obj, value);
			}

			// Draw the field
			var so = new SerializedObject(obj);
			var prop = so.FindProperty("value");
			var ret = EditorGUI.GetPropertyHeight(prop, label);
			UnityEngine.Object.DestroyImmediate(obj);

			return ret;
		}

		private static ScriptableObject EmitScriptableObject(Type t, Type emit)
		{
			if (emitScriptableObjectCache == null)
			{
				emitScriptableObjectCache = new Dictionary<Type, ScriptableObject>();
			}
			if (emitScriptableObjectCache.ContainsKey(t))
			{
				if (Util.IsNull(emitScriptableObjectCache[t]))
				{
					emitScriptableObjectCache.Remove(t);
				}
				else
				{
					return emitScriptableObjectCache[t];
				}
			}

			var obj = ScriptableObject.CreateInstance(emit);
			emitScriptableObjectCache[t] = obj;
			return obj;
		}

		private static Type EmitType(Type t)
		{
			if (emitTypeCache == null)
			{
				emitTypeCache = new Dictionary<Type, Type>();
			}
			if (emitTypeCache.ContainsKey(t))
			{
				if (Util.IsNull(emitTypeCache[t]))
				{
					emitTypeCache.Remove(t);
				}
				else
				{
					return emitTypeCache[t];
				}
			}

			// Create a mock type
			var an = new AssemblyName("NodeGraphEmit");
			var ad = AppDomain.CurrentDomain;
			var ab = ad.DefineDynamicAssembly(an,
				AssemblyBuilderAccess.Run);
			var mb = ab.DefineDynamicModule(an.Name);
			var tb = mb.DefineType("Temp",
				TypeAttributes.Public | TypeAttributes.Class,
				typeof(ScriptableObject));

			tb.DefineField("value", t, FieldAttributes.Public);

			// Create the type
			var type = tb.CreateType();
			emitTypeCache[t] = type;
			return type;
		}
	}
}