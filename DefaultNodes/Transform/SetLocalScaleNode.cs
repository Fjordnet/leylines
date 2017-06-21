using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Set Local Scale", Path = "Transform/Set Local Scale")]
	public class SetScaleNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The transform to set the scale of.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("The scale to use.")]
		[SerializeField, Input("Scale")]
		internal Vector3 scale = Vector3.one;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;
		[Description("The transform used, for chaining.")]
		[SerializeField, Output("Transform")]
		internal Transform transformOut = null;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			transform.localScale = scale;

			transformOut = transform;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
