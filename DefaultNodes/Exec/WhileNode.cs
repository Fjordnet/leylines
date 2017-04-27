using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "While", Path = "Exec/While")]
	public class WhileNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;

		[Description("True if the signal should continue.")]
		[SerializeField, Input("Condition", 20, SocketFlags.Editable)]
		internal bool condition = false;

		[Description("The output signal, if the condition was true.")]
		[SerializeField, Output("True")]
		internal ExecType execTrue = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			while (condition)
			{
				// Create a copy of the scope to force re-eval every time this
				// loops
				var newScope = new GraphScope(scope);

				yield return new SignalSocket(newScope, GetSocket("execTrue"));

				// Force re-evaluation of this node
				Eval(scope);
			}
		}

		#endregion
	}
}
