using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Set Rotation", Path = "Transform/Set Rotation")]
	public class SetRotationNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The transform to set the rotation of.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("Defines the rotation in local or world space.")]
		[SerializeField, Input("Relative To")]
		internal Space relativeTo = Space.Self;
		[Description("The rotation to use, in degrees.")]
		[SerializeField, Input("Rotation")]
		internal Vector3 rotation = Vector3.zero;

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
				transform.localRotation = Quaternion.Euler(rotation);
			}
			else
			{
				transform.rotation = Quaternion.Euler(rotation);
			}

			transformOut = transform;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
