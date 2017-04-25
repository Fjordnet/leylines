using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "RotateAround", Path = "Transform/Rotate Around")]
	public class RotateAroundNode : BakedNode
	{
		[SerializeField, Input("Exec", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[SerializeField, Input("Relative To")]
		internal Space space = Space.Self;
		[SerializeField, Input("Point")]
		internal Vector3 point = Vector3.zero;
		[SerializeField, Input("Axis")]
		internal Vector3 axis = Vector3.up;
		[SerializeField, Input("Degrees")]
		internal float degrees = 0;
		[SerializeField, Input("Delta")]
		internal float delta = 1;

		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;
		[SerializeField, Output("Transform")]
		internal Transform transformOut = null;
		[SerializeField, Output("Delta")]
		internal float deltaOut = 1;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			var point = this.point;
			if (space == Space.Self) {
				point += transform.position;
			}

			transform.RotateAround(point, axis, degrees * delta);
			transformOut = transform;
			deltaOut = delta;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
