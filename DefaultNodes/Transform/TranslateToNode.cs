using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Translate To", Path = "Transform/Translate To")]
	public class TranslateToNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The transform to translate.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("Defines the position in local or world space.")]
		[SerializeField, Input("Relative To")]
		internal Space relativeTo = Space.Self;
		[Description("The position to go to.")]
		[SerializeField, Input("Destination")]
		internal Vector3 destination = Vector3.zero;

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
			if (relativeTo == Space.Self)
			{
				transform.localPosition = destination;
			}
			else
			{
				transform.position = destination;
			}

			transformOut = transform;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
