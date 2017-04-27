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
				yield return new SignalSocket(scope, GetSocket("execTrue"));

				// Force re-evaluation
				Eval(scope);
			}
		}

		#endregion
	}
}
