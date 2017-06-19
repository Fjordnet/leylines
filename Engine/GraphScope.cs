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
using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	public class GraphScope
	{
		public GameObject GameObject { get; set; }
		public Dictionary<Socket, object> Values { get; set; }
		public Dictionary<string, DynamicValue> Vars { get; set; }

		private HashSet<int> evaluatedIDs;

		public GraphScope(GameObject self, Graph graph)
		{
			GameObject = self;
			Values = new Dictionary<Socket, object>();
			Vars = new Dictionary<string, DynamicValue>();
			evaluatedIDs = new HashSet<int>();

			foreach (var v in graph.Variables)
			{
				Vars.Add(v.Name, v.Value);
			}
		}

		/// <summary>
		/// Creates a deep copy of another scope.
		/// </summary>
		public GraphScope(GraphScope other)
		{
			Values = new Dictionary<Socket, object>(other.Values);
			evaluatedIDs = new HashSet<int>(other.evaluatedIDs);

			foreach (var v in other.Vars)
			{
				Vars.Add(v.Key, v.Value);
			}
		}

		public bool ShouldEvaluate(int nodeID)
		{
			return evaluatedIDs.Add(nodeID);
		}
	}
}
