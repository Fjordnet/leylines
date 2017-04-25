using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Gate", Path = "Exec/Gate")]
	public class GateNode : BakedNode
	{
		[Description("The execution signal that opens the gate.")]
		[SerializeField, Input("Open", SocketFlags.AllowMultipleLinks)]
		internal ExecType open = ExecType.None;

		[Description("The execution signal that closes the gate.")]
		[SerializeField, Input("Close", SocketFlags.AllowMultipleLinks)]
		internal ExecType close = ExecType.None;

		[Description("The execution signal to pass through the gate.")]
		[SerializeField, Input("Exec In", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;

		[Description("True if the gate is open.")]
		[SerializeField, Input("IsOpen", 20, SocketFlags.Editable)]
		internal bool isOpen = false;

		[Description("The execution signal that passed through the gate.")]
		[SerializeField, Output("Exec Out")]
		internal ExecType execOut = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			if (to.FieldName == "open")
			{
				isOpen = true;
			}
			else if (to.FieldName == "close")
			{
				isOpen = false;
			}
			else if (isOpen)
			{
				yield return new SignalSocket(scope, GetSocket("execOut"));
			}
			else
			{
				yield return null;
			}
		}

		#endregion
	}
}
