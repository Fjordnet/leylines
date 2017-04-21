using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Wait For Seconds", Path = "Exec/Wait For Seconds")]
	public class WaitForSecondsNode : BakedNode
	{
		/// <summary>
		/// The input signal.
		/// </summary>
		[SerializeField, Input("Exec", false, true)]
		internal ExecType execIn = ExecType.None;

		/// <summary>
		/// The number of seconds to wait for.
		/// </summary>
		[SerializeField, Input("Seconds", 50)]
		internal float seconds = 1;

		/// <summary>
		/// The output signal.
		/// </summary>
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
