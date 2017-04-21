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
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[Serializable]
	public class DynamicValue
	{
		[HideInInspector, SerializeField]
		private string typeStr;
		[HideInInspector, SerializeField]
		private UnityEngine.Object obj;
		[HideInInspector, SerializeField]
		private string str;

		public object Value
		{
			get
			{
				// Deserialize value
				if (!string.IsNullOrEmpty(typeStr))
				{
					var type = Type.GetType(typeStr);

					// Unknown type
					if (type == null)
					{
						return null;
					}

					// UnityEngine.Object type
					if (typeof(UnityEngine.Object).IsAssignableFrom(type))
					{
						return obj;
					}

					// Custom type
					return DeserializeValue(str, type);
				}

				return null;
			}
			set
			{
				// Null value
				if (Util.IsNull(value)) {
					obj = null;
					str = null;
					return;
				}

				// UnityEngine.Object value
				if (value is UnityEngine.Object)
				{
					obj = (UnityEngine.Object)value;
				}

				// Custom value
				else
				{
					str = SerializeValue(value);
				}
			}
		}

		public Type Type
		{
			get
			{
				return Type.GetType(typeStr);
			}
			set
			{
				// Serialize type
				if (value == null)
				{
					typeStr = null;
					return;
				}

				typeStr = value.FullName + ", " + value.Assembly.GetName().Name;
				return;
			}
		}

		public static string SerializeValue(object value)
		{
			var serializer = new XmlSerializer(value.GetType());
			using (var stream = new StringWriter())
			{
				serializer.Serialize(stream, value);
				var str = stream.ToString();
				return str;
			}
		}

		public static object DeserializeValue(string str, Type type)
		{
			if (string.IsNullOrEmpty(str))
			{
				return null;
			}

			var serializer = new XmlSerializer(type);
			using (var stream = new StringReader(str))
			{
				try
				{
					return serializer.Deserialize(stream);
				}
				// Mismatched type failure
				catch (Exception)
				{
					return null;
				}
			}
		}
	}
}
