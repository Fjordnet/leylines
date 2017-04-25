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
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A helper for the graph player that runs coroutines on the player's
	/// behalf.
	/// </summary>
	[AddComponentMenu("")]
	public class NodeGraphPlayer : MonoBehaviour
	{
		#region Properties

		public NodeGraph NodeGraph
		{
			get { return nodeGraph; }
			set
			{
				nodeGraph = value;
				CacheEntryPoints();
			}
		}

		private Graph Graph
		{
			get { return Util.IsNull(nodeGraph) ? null : nodeGraph.Graph; }
		}

		#endregion

		private NodeGraph nodeGraph;
		private List<IEnumerator<Yield>> yields;
		private bool destroyWhenDone = false;

		#region Logic

		public void Exec(ExecType type)
		{
			if (type == ExecType.OnDestroy)
			{
				destroyWhenDone = true;
			}

			if (Util.IsNull(Graph))
			{
				return;
			}

			if (yields == null)
			{
				yields = new List<IEnumerator<Yield>>();
			}

			foreach (var from in entryPoints[type])
			{
				// Initialize the scope
				var scope = new GraphScope();
				Eval(from.GetNode(Graph), scope);

				// Start Execution
				foreach (var to in Graph.Links.HasSocketAsSource(from))
				{
					var yield = Exec(from, to, scope);
					while (yield.MoveNext())
					{
						// Check if execution is delayed
						if (yield.Current != null && !yield.Current.Finished)
						{
							yields.Add(yield);
							break;
						}
					}
				}
			}
		}

		private IEnumerator<Yield> Exec(Socket from, Socket to, GraphScope scope)
		{
			var source = to.GetNode(Graph);
			var destination = to.GetNode(Graph);

			// Store from socket outputs in the scope
			foreach (var output in source.GetOutputSockets())
			{
				scope.Values[output] = source.GetSocketValue(output);
			}

			// Evaluate the next node
			Eval(destination, scope);

			// Execute the next node
			var yield = destination.Exec(from, to, scope);
			while (yield.MoveNext()) yield return yield.Current;

			// Check if execution is finished
			if (yield.Current == null)
			{
				yield break;
			}

			var signal = yield.Current as SignalSocket;
			if (signal == null)
			{
				yield break;
			}

			// Continue execution
			var newFrom = signal.Socket;
			foreach (var newTo in Graph.Links.HasSocketAsSource(newFrom))
			{
				yield = Exec(newFrom, newTo, scope);
				while (yield.MoveNext()) yield return yield.Current;
			}
		}

		private void Eval(Node toEval, GraphScope scope)
		{
			var sockets = toEval.GetInputSockets();
			foreach (var socket in sockets)
			{
				var inputs = Graph.Links.HasSocketAsDestination(socket);
				foreach (var input in inputs)
				{
					// Evaluate the node
					Node node;
					try
					{
						node = input.GetNode(Graph);
					}
					catch (Exception e)
					{
						Debug.LogWarningFormat(
							"Could not evaluate {0} in {1}:\n{2}",
							input, Graph, e);
						continue;
					}

					if (scope.ShouldEvaluate(node.ID))
					{
						Eval(input.GetNode(Graph), scope);

						// Store source socket outputs in the scope
						foreach (var output in node.GetOutputSockets())
						{
							scope.Values[output] = node.GetSocketValue(output);
						}
					}

					// Set the input destination socket values
					toEval.SetSocketValue(socket, scope.Values[input]);
				}
			}

			toEval.Eval(scope);
		}

		#endregion

		#region Monobehaviour

		public void Update()
		{
			if (yields == null)
			{
				if (destroyWhenDone)
				{
					Destroy(gameObject);
				}
				return;
			}

			var toRemove = new List<IEnumerator<Yield>>();

			foreach (var yield in yields)
			{
				if (yield.Current == null)
				{
					toRemove.Add(yield);
					continue;
				}

				yield.Current.OnUpdate();

				if (!yield.Current.Finished)
				{
					continue;
				}

				if (!yield.MoveNext())
				{
					toRemove.Add(yield);
					continue;
				}
			}

			foreach (var remove in toRemove)
			{
				yields.Remove(remove);
			}

			if (destroyWhenDone && yields.Count == 0)
			{
				Destroy(gameObject);
			}
		}

		#endregion

		#region Util

		Dictionary<ExecType, List<Socket>> entryPoints;

		private void CacheEntryPoints()
		{
			if (entryPoints == null)
			{
				entryPoints = new Dictionary<ExecType, List<Socket>>();
			}
			entryPoints.Clear();

			// Init cache data structure
			foreach (ExecType type in Enum.GetValues(typeof(ExecType)))
			{
				if (type == ExecType.None)
				{
					continue;
				}
				entryPoints.Add(type, new List<Socket>());
			}

			foreach (var node in Graph.Nodes)
			{
				if (Util.IsNull(node))
				{
					continue;
				}

				foreach (var socket in node.GetOutputSockets())
				{
					if (Util.IsNull(socket))
					{
						continue;
					}

					if (typeof(ExecType) != socket.GetSocketType(Graph))
					{
						continue;
					}

					var value = (ExecType?)node.GetSocketValue(socket);
					if (value == null || value == ExecType.None)
					{
						continue;
					}

					entryPoints[value.Value].Add(socket);
				}
			}
		}

		#endregion
	}
}