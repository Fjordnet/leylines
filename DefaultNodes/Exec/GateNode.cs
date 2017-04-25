using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Gate", Path = "Exec/Gate")]
	public class GateNode : BakedNode
	{
		/// <summary>
		/// Opens the gate
		/// </summary>
		[SerializeField, Input("Open", SocketFlags.AllowMultipleLinks)]
		internal ExecType open = ExecType.None;

		/// <summary>
		/// Closes the gate
		/// </summary>
		[SerializeField, Input("Close", SocketFlags.AllowMultipleLinks)]
		internal ExecType close = ExecType.None;

		[SerializeField, Input("Exec In", SocketFlags.AllowMultipleLinks)]
		internal ExecType execIn = ExecType.None;

		[SerializeField, Input("IsOpen", SocketFlags.Editable)]
		internal bool isOpen = false;

		[SerializeField, Output("Exec Out", SocketFlags.AllowMultipleLinks)]
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
