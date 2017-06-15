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
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A bi-directional mapping of sockets.
	/// </summary>
	[Serializable]
	public class VariableDictionary : IEnumerable<Variable>
	{
		[SerializeField]
		private List<Variable> variables = new List<Variable>();

		public VariableDictionary() { }

		public VariableDictionary(List<Variable> variables)
		{
			this.variables = variables;
		}

		#region Operations

		public bool Add(string name, DynamicValue value)
		{
			InitCache();

			if (name == null) 
			{
				throw new ArgumentNullException("name");
			}
			if (value == null) 
			{
				throw new ArgumentNullException("value");
			}

			if (cache.ContainsKey(name))
			{
				return false;
			}

			variables.Add(new Variable(name, value));
			cache.Add(name, value);
			return true;
		}

		public bool Remove(string name)
		{
			InitCache();

			if (!cache.ContainsKey(name))
			{
				return false;
			}

			var count = variables.RemoveAll(x => x.Name == name);
			cache.Remove(name);
			return count > 0;
		}

		#endregion

		#region Checks

		/// <summary>
		/// Returns true if the link specified by the two sockets exists.
		/// </summary>
		/// <param name="graph">The graph these sockets are in.</param>
		/// <param name="a">The first socket in the link.</param>
		/// <param name="b">The second socket in the link.</param>
		/// <returns>True if that link specified by the two sockets exists.</returns>
		public bool Contains(string name)
		{
			InitCache();
			return cache.ContainsKey(name);
		}

		#endregion

		IEnumerator IEnumerable.GetEnumerator()
		{
			return variables.GetEnumerator();
		}

		public IEnumerator<Variable> GetEnumerator()
		{
			return variables.GetEnumerator();
		}

		#region Util

		private Dictionary<string, DynamicValue> cache;
		private void InitCache()
		{
			if (cache == null)
			{
				cache = new Dictionary<string, DynamicValue>();

				foreach (var variable in variables)
				{
					cache[variable.Name] = variable.Value;
				}
			}
		}

		public List<Variable> AsList()
		{
			return variables;
		}

		#endregion
	}
}
