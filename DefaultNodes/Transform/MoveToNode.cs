using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Move To", Path = "Transform/Move To")]
	public class MoveToNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", (SocketFlags)0)]
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
		[Description("The amount of time in seconds to move.")]
		[SerializeField, Input("Seconds")]
		internal float seconds = 1;
		[Description("The easing to use.")]
		[SerializeField, Input("Easing")]
		internal Easings.Easing easing = Easings.Easing.Linear;
		[Description("If true, ignore time scaling.")]
		[SerializeField, Input("Ignore Time Scale")]
		internal bool ignoreTimeScale = false;

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
			Vector3 a, b;
			var elapsed = 0f;

			switch (relativeTo)
			{
				case Space.Self:
					a = transform.localPosition;
					b = destination;

					while (elapsed < seconds)
					{
						var t = elapsed / seconds;
						t = easing.Interpolate(t);
						transform.localPosition = Vector3.LerpUnclamped(a, b, t);

						yield return new WaitForUpdate();

						elapsed += ignoreTimeScale
							? Time.deltaTime : Time.unscaledDeltaTime;
					}

					transform.localPosition = b;

					break;

				case Space.World:
					a = transform.position;
					b = destination;

					while (elapsed < seconds)
					{
						var t = elapsed / seconds;
						t = easing.Interpolate(t);
						transform.position = Vector3.LerpUnclamped(a, b, t);

						yield return new WaitForUpdate();

						elapsed += ignoreTimeScale
							? Time.deltaTime : Time.unscaledDeltaTime;
					}

					transform.position = b;
					break;
			}

			transformOut = transform;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
