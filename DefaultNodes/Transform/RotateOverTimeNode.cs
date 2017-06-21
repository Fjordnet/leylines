using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Rotate Over Time", Path = "Transform/Rotate Over Time")]
	public class RotateOverTimeNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The transform to rotate.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("Defines the rotation in local or world space.")]
		[SerializeField, Input("Relative To")]
		internal Space relativeTo = Space.Self;
		[Description("The axis of the rotation.")]
		[SerializeField, Input("Axis")]
		internal Vector3 axis = Vector3.up;
		[Description("The number of degrees to rotate by.")]
		[SerializeField, Input("Degrees")]
		internal float degrees = 0;
		[Description("The amount of time in seconds to take.")]
		[SerializeField, Input("Seconds")]
		internal float seconds = 1;
		[Description("The easing to use.")]
		[SerializeField, Input("Easing")]
		internal Easings.Easing easing = Easings.Easing.Linear;
		[Description("If true, ignore time scaling.")]
		[SerializeField, Input("Ignore Time Scale")]
		internal bool ignoreTimeScale = false;
		[Description("If true, wait until done moving before invoking the output signal.")]
		[SerializeField, Input("Wait Until Done")]
		internal bool waitUntilDone = false;

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
			var agent = new GameObject().AddComponent<RotateAgent>();
			agent.target = transform;
			agent.relativeTo = relativeTo;
			agent.axis = axis;
			agent.degrees = degrees;
			agent.seconds = seconds;
			agent.easing = easing;
			agent.ignoreTimeScale = ignoreTimeScale;
			agent.Move();

			if (waitUntilDone)
			{
				while (!Util.IsNull(agent))
				{
					yield return new WaitForUpdate();
				}
			}

			transformOut = transform;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion

		private class RotateAgent : MonoBehaviour
		{
			public Transform target = null;
			public Space relativeTo = Space.Self;
			public Vector3 axis = Vector3.up;
			public float degrees = 0;
			public float seconds = 1;
			public Easings.Easing easing = Easings.Easing.Linear;
			public bool ignoreTimeScale = false;

			private void Awake()
			{
				gameObject.hideFlags = HideFlags.HideAndDontSave;
			}

			public void Move()
			{
				StartCoroutine(RotateRoutine());
			}

			private IEnumerator RotateRoutine()
			{
				Quaternion a, b;
				var elapsed = 0f;

				switch (relativeTo)
				{
					case Space.Self:
						a = transform.localRotation;
						b = a * Quaternion.AngleAxis(degrees, axis);

						while (elapsed < seconds)
						{
							var t = elapsed / seconds;
							t = easing.Interpolate(t);

							target.localRotation =
								a * Quaternion.AngleAxis(degrees * t, axis);

							yield return new WaitForEndOfFrame();

							elapsed += ignoreTimeScale
								? Time.deltaTime : Time.unscaledDeltaTime;
						}

						target.localRotation = b;

						break;

					case Space.World:
						a = transform.rotation;
						b = a * Quaternion.AngleAxis(degrees, axis);

						while (elapsed < seconds)
						{
							var t = elapsed / seconds;
							t = easing.Interpolate(t);

							target.rotation =
								a * Quaternion.AngleAxis(degrees * t, axis);

							yield return new WaitForEndOfFrame();

							elapsed += ignoreTimeScale
								? Time.deltaTime : Time.unscaledDeltaTime;
						}

						target.rotation = b;

						break;
				}

				DestroyImmediate(gameObject);
			}
		}
	}
}
