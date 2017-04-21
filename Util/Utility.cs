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

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// Wraps multiple undo commands into one.
	/// </summary>
	public class UndoStack : System.IDisposable
	{
		private int group { get; set; }

		/// <summary>
		/// Creates a new undo stack with the specified group name.
		/// </summary>
		/// <param name="label">The name to use for this undo group.</param>
		public UndoStack(string label)
		{
			Undo.SetCurrentGroupName(label);
			group = Undo.GetCurrentGroup();
			Undo.IncrementCurrentGroup();
		}

		public void Dispose()
		{
			Undo.CollapseUndoOperations(group);
		}
	}

	public static partial class Util
	{
		/// <summary>
		/// Returns true if the object is null or is a GameObject that has
		/// been destroyed.
		/// </summary>
		/// <param name="obj">The object to check.</param>
		/// <returns>
		/// True if the object is null or is a GameObject that has been
		/// destroyed.
		/// </returns>
		public static bool IsNull(object obj)
		{
			return obj == null || obj.Equals(null);
		}

		/// <summary>
		/// Calculates a hash code for multiple objects.
		/// </summary>
		/// <param name="args">The objects to hash.</param>
		/// <returns>A hash code for multiple objects.</returns>
		public static int GetHashCode(params object[] args)
		{
			unchecked
			{
				int hash = 27;
				foreach (var arg in args)
				{
					hash = (13 * hash);

					if (arg != null)
					{
						hash += arg.GetHashCode();
					}
				}
				return hash;
			}
		}
	}
}
