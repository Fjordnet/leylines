using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Wait For Seconds", Path = "Exec/Wait For Seconds")]
	public class WaitForSecondsNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;

		[Description("The number of seconds to wait for.")]
		[SerializeField, Input("Seconds", 50)]
		internal float seconds = 1;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			yield return new WaitForSeconds(seconds);
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
