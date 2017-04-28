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

namespace Exodrifter.NodeGraph
{
	public abstract class BakedNode : Node
	{
		#region Properties

		public override string DisplayName
		{
			get
			{
				CacheNodeAttributeData();
				return cachedDisplayName;
			}
			set { return; }
		}

		#endregion

		public override Socket[] GetInputSockets()
		{
			CacheSocketAttributeData();
			return inputsCache;
		}

		public override Socket[] GetOutputSockets()
		{
			CacheSocketAttributeData();
			return outputsCache;
		}

		public override Socket GetSocket(string name)
		{
			return new Socket(ID, name);
		}

		public override Socket[] GetSockets()
		{
			CacheSocketAttributeData();
			return socketsCache;
		}

		#region Socket Properties

		public override string GetSocketDescription(string name)
		{
			CacheDescriptionAttributeData();
			return descriptionCache[name];
		}

		public override string GetSocketDisplayName(string name)
		{
			CacheSocketAttributeData();
			return socketAttrCache[name].name;
		}

		public override SocketFlags GetSocketFlags(string name)
		{
			CacheSocketAttributeData();
			return socketAttrCache[name].flags;
		}

		public override Type GetSocketType(string name)
		{
			var field = GetField(name);
			return field.FieldType;
		}

		public override object GetSocketValue(string name)
		{
			var field = GetField(name);
			return field.GetValue(this);
		}

		public override bool IsSocketInput(string name)
		{
			CacheSocketAttributeData();
			return socketIsInputCache[name];
		}

		public override void SetSocketValue(string name, object value)
		{
			var field = GetField(name);
			field.SetValue(this, value);
		}

		#endregion

		#region Util

		private Dictionary<string, FieldInfo> fieldCache;
		private FieldInfo GetField(string name)
		{
			fieldCache = fieldCache ?? new Dictionary<string, FieldInfo>();
			if (fieldCache.ContainsKey(name))
			{
				return fieldCache[name];
			}

			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var field = GetType().GetField(name, flags);

			var attrs = field.GetCustomAttributes(true);
			foreach (var attr in attrs)
			{
				if (attr is SocketAttribute)
				{
					fieldCache[name] = field;
					return field;
				}
			}

			fieldCache[name] = null;
			return null;
		}

		private Dictionary<Type, FieldInfo[]> fieldsCache;
		private Dictionary<Type, Attribute[]> attrCache;
		private FieldInfo[] GetFields<T>(out T[] attributes) where T : Attribute
		{
			fieldsCache = fieldsCache ?? new Dictionary<Type, FieldInfo[]>();
			attrCache = attrCache ?? new Dictionary<Type, Attribute[]>();
			if (fieldsCache.ContainsKey(typeof(T)))
			{
				attributes = Array.ConvertAll(attrCache[typeof(T)], x => (T)x);
				return fieldsCache[typeof(T)];
			}

			var retFields = new List<FieldInfo>();
			var retAttrs = new List<T>();

			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var fields = GetType().GetFields(flags);

			foreach (var field in fields)
			{
				var attrs = field.GetCustomAttributes(true);
				foreach (var attr in attrs)
				{
					if (typeof(T).IsAssignableFrom(attr.GetType()))
					{
						retAttrs.Add((T)attr);
						retFields.Add(field);
					}
				}
			}

			attributes = retAttrs.ToArray();
			fieldsCache[typeof(T)] = retFields.ToArray();
			attrCache[typeof(T)] = attributes;
			return fieldsCache[typeof(T)];
		}

		#endregion

		#region Caching Methods

		private string cachedDisplayName;
		private void CacheNodeAttributeData()
		{
			if (cachedDisplayName != null)
			{
				return;
			}

			var type = GetType();
			var attributes = (NodeAttribute[])
			Attribute.GetCustomAttributes(type, typeof(NodeAttribute));
			if (attributes.Length == 0)
			{
				cachedDisplayName = GetType().Name;
			}
			else
			{
				cachedDisplayName = attributes[0].Name;
			}
		}

		private Socket[] socketsCache;
		private Socket[] inputsCache;
		private Socket[] outputsCache;
		private Dictionary<string, bool> socketIsInputCache;
		private Dictionary<string, FieldInfo> socketFieldCache;
		private Dictionary<string, SocketAttribute> socketAttrCache;
		private void CacheSocketAttributeData()
		{
			if (socketFieldCache != null && socketAttrCache != null
				&& socketsCache != null && inputsCache != null
				&& outputsCache != null)
			{
				return;
			}

			socketIsInputCache = new Dictionary<string, bool>();
			socketFieldCache = new Dictionary<string, FieldInfo>();
			socketAttrCache = new Dictionary<string, SocketAttribute>();

			var sockets = new List<Socket>();
			var inputs = new List<Socket>();
			var outputs = new List<Socket>();

			SocketAttribute[] attrs;
			var fields = GetFields(out attrs);
			for (int i = 0; i < fields.Length; ++i)
			{
				var name = fields[i].Name;
				var isInput = attrs[i] is InputAttribute;

				socketIsInputCache[name] = isInput;
				socketFieldCache[name] = fields[i];
				socketAttrCache[name] = attrs[i];

				var socket = new Socket(ID, name);
				sockets.Add(socket);
				if (isInput)
				{
					inputs.Add(socket);
				}
				else
				{
					outputs.Add(socket);
				}
			}

			socketsCache = sockets.ToArray();
			inputsCache = inputs.ToArray();
			outputsCache = outputs.ToArray();
		}

		Dictionary<string, string> descriptionCache;
		private void CacheDescriptionAttributeData()
		{
			if (descriptionCache != null)
			{
				return;
			}

			descriptionCache = new Dictionary<string, string>();

			DescriptionAttribute[] attrs;
			var fields = GetFields(out attrs);
			for (int i = 0; i < fields.Length; ++i)
			{
				descriptionCache[fields[i].Name] = attrs[i].text;
			}
		}

		#endregion
	}
}
