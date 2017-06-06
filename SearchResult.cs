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

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// Represents a result in the search panel that, when picked, will add
	/// a new node to the graph.
	/// </summary>
	public class SearchResult
	{
		/// <summary>
		/// The string to display in the search panel.
		/// </summary>
		public string Label { get; set;  }

		/// <summary>
		/// The function to execute when this result is selected.
		/// </summary>
		public Func<Node> MakeNode { get; set; }

		/// <summary>
		/// The inputs on the node this search result will make.
		/// </summary>
		public List<Type> Inputs;

		/// <summary>
		/// The outputs on the node this search result will make.
		/// </summary>
		public List<Type> Outputs;

		public SearchResult(string label, Func<Node> makeNode,
			List<Type> inputs = null, List<Type> outputs = null)
		{
			Label = label;
			MakeNode = makeNode;
			Inputs = inputs ?? new List<Type>();
			Outputs = outputs ?? new List<Type>();
		}

		/// <summary>
		/// Returns true if the specified context matches this result.
		/// </summary>
		/// <param name="contextIsInput">
		/// True if the context type is an input.
		/// </param>
		/// <param name="contextType">
		/// The type to check for.
		/// </param>
		/// <returns>
		/// True if the context type matches the inputs or outputs.
		/// </returns>
		public bool MatchesContext(bool contextIsInput, Type contextType)
		{
			if (contextIsInput)
			{
				return Inputs != null && Inputs.Contains(contextType);
			}
			else
			{
				return Outputs != null && Outputs.Contains(contextType);
			}
		}
	}
}
