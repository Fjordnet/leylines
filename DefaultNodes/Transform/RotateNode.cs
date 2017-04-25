using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Rotate", Path = "Transform/Rotate")]
	public class RotateNode : BakedNode
	{
		[SerializeField, Input("Exec", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[SerializeField, Input("Relative To")]
		internal Space space = Space.Self;
		[SerializeField, Input("Degrees")]
		internal Vector3 degrees = Vector3.zero;
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
			transform.Rotate(degrees * delta, space);
			transformOut = transform;
			deltaOut = delta;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
