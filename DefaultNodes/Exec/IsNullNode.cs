using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Is Null", Path = "Exec/Is Null")]
	public class IsNullNode : BakedNode
	{
		[Description("The execution signal that performs the test.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;

		[Description("The object to check.")]
		[SerializeField, Input("Object")]
		internal Object obj = null;

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
			if (Util.IsNull(obj))
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
