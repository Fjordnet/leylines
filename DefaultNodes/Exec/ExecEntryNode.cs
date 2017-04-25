using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Execution", Path = "Exec/ExecEntry")]
	public class ExecEntryNode : BakedNode
	{
		[Description("The execution signal to trigger on.")]
		[SerializeField, Output("Exec", SocketFlags.AllowMultipleLinks | SocketFlags.Editable)]
		internal ExecType execOut = ExecType.OnStart;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
