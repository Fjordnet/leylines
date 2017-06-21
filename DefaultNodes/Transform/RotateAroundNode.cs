using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "RotateAround", Path = "Transform/Rotate Around")]
	public class RotateAroundNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;
		[Description("The transform to rotate.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("The point in world space to rotate around.")]
		[SerializeField, Input("Point")]
		internal Vector3 point = Vector3.zero;
		[Description("The axis through the point to rotate around.")]
		[SerializeField, Input("Axis")]
		internal Vector3 axis = Vector3.up;
		[Description("The number of degrees to rotate by.")]
		[SerializeField, Input("Degrees")]
		internal float degrees = 0;
		[Description("The delta to scale the degrees by.")]
		[SerializeField, Input("Delta")]
		internal float delta = 1;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;
		[Description("The transform used, for chaining.")]
		[SerializeField, Output("Transform")]
		internal Transform transformOut = null;
		[Description("The delta used, for chaining.")]
		[SerializeField, Output("Delta")]
		internal float deltaOut = 1;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			transform.RotateAround(point, axis, degrees * delta);
			transformOut = transform;
			deltaOut = delta;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
