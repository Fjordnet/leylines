using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "If", Path = "Exec/If")]
	public class IfNode : BakedNode
	{
		[Description("The execution signal that performs the test.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;

		[Description("True if the signal should continue.")]
		[SerializeField, Input("Condition", SocketFlags.Editable)]
		internal bool condition = false;

		[Description("The execution signal, if the condition was true.")]
		[SerializeField, Output("True")]
		internal ExecType execTrue = ExecType.None;

		[Description("The execution signal, if the condition was false.")]
		[SerializeField, Output("False")]
		internal ExecType execFalse = ExecType.None;

		public override float InputWidth { get { return 30; } }

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			if (condition)
			{
				yield return new SignalSocket(scope, GetSocket("execTrue"));
			}
			else
			{
				yield return new SignalSocket(scope, GetSocket("execFalse"));
			}
		}

		#endregion
	}
}
