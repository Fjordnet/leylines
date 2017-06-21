using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Set Enabled", Path = "Object/Set Enabled")]
	public class SetEnabledNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The Behaviour or Component to disable or enable.")]
		[SerializeField, Input("Behaviour")]
		internal Behaviour behaviour = null;
		[Description("True if the Behaviour or Component should be enabled.")]
		[SerializeField, Input("Enable")]
		internal bool enable = false;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			behaviour.enabled = enable;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
