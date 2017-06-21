using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Set Active", Path = "Object/Set Active")]
	public class SetActiveNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The GameObject to deactivate or activate.")]
		[SerializeField, Input("GameObject")]
		internal GameObject gameObject = null;
		[Description("True if the GameObject should be activated.")]
		[SerializeField, Input("Active")]
		internal bool active = false;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			gameObject.SetActive(active);
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
