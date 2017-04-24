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

		public string ContextPath
		{
			get
			{
				if (string.IsNullOrEmpty(cachedContextPath))
				{
					CacheAttributeData();
				}
				return cachedContextPath;
			}
		}
		private string cachedContextPath;

		public override string DisplayName
		{
			get
			{
				if (string.IsNullOrEmpty(cachedDisplayName))
				{
					CacheAttributeData();
				}
				return cachedDisplayName;
			}
			set { return; }
		}
		private string cachedDisplayName;

		#endregion

		public override Socket[] GetInputSockets()
		{
			var ret = new List<Socket>();
			var fields = GetFields<InputAttribute>();

			foreach (var field in fields)
			{
				ret.Add(new Socket(ID, field.Name));
			}

			return ret.ToArray();
		}

		public override Socket[] GetOutputSockets()
		{
			var ret = new List<Socket>();
			var fields = GetFields<OutputAttribute>();

			foreach (var field in fields)
			{
				ret.Add(new Socket(ID, field.Name));
			}

			return ret.ToArray();
		}

		public override Socket GetSocket(string name)
		{
			return new Socket(ID, name);
		}

		public override Socket[] GetSockets()
		{
			var ret = new List<Socket>();
			var fields = GetFields<SocketAttribute>();

			foreach (var field in fields)
			{
				ret.Add(new Socket(ID, field.Name));
			}

			return ret.ToArray();
		}

		#region Socket Properties

		public override string GetSocketDisplayName(string name)
		{
			var attr = GetFieldAttribute<SocketAttribute>(name);
			return attr.name;
		}

		public override SocketFlags GetSocketFlags(string name)
		{
			var attr = GetFieldAttribute<SocketAttribute>(name);
			return attr.flags;
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

		public override int GetSocketWidth(string name)
		{
			var attr = GetFieldAttribute<SocketAttribute>(name);
			return attr.width;
		}

		public override bool IsSocketInput(string name)
		{
			var attr = GetFieldAttribute<SocketAttribute>(name);
			return attr is InputAttribute;
		}

		public override void SetSocketValue(string name, object value)
		{
			var field = GetField(name);
			field.SetValue(this, value);
		}

		#endregion

		#region Util

		private FieldInfo GetField(string name)
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var field = GetType().GetField(name, flags);

			var attrs = field.GetCustomAttributes(true);
			foreach (var attr in attrs)
			{
				if (attr is SocketAttribute)
				{
					return field;
				}
			}

			return null;
		}

		private FieldInfo[] GetFields<T>() where T : Attribute
		{
			var ret = new List<FieldInfo>();

			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var fields = GetType().GetFields(flags);

			foreach (var field in fields)
			{
				var attrs = field.GetCustomAttributes(true);
				foreach (var attr in attrs)
				{
					if (typeof(T).IsAssignableFrom(attr.GetType()))
					{
						ret.Add(field);
					}
				}
			}

			return ret.ToArray();
		}

		private T GetFieldAttribute<T>(string name) where T : Attribute
		{
			var flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var matches = GetType().GetMember(name, flags);

			foreach (var match in matches)
			{
				var attrs = match.GetCustomAttributes(true);
				foreach (var attr in attrs)
				{
					if (typeof(T).IsAssignableFrom(attr.GetType()))
					{
						return (T)attr;
					}
				}
			}

			return null;
		}

		private void CacheAttributeData()
		{
			var type = GetType();
			var attributes = (NodeAttribute[])
				Attribute.GetCustomAttributes(type, typeof(NodeAttribute));
			if (attributes.Length == 0)
			{
				cachedDisplayName = GetType().Name;
				cachedContextPath = "Other/" + GetType().Name;
			}
			else
			{
				cachedDisplayName = attributes[0].Name;
				cachedContextPath = attributes[0].Path;
			}
		}

		#endregion
	}
}
