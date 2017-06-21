using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Translate To Over Time", Path = "Transform/Translate To Over Time")]
	public class TranslateToOverTimeNode : BakedNode
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
			var agent = new GameObject().AddComponent<TranslateToAgent>();
			agent.target = transform;
			agent.relativeTo = relativeTo;
			agent.destination = destination;
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

		private class TranslateToAgent : MonoBehaviour
		{
			public Transform target = null;
			public Space relativeTo = Space.Self;
			public Vector3 destination = Vector3.zero;
			public float seconds = 1;
			public Easings.Easing easing = Easings.Easing.Linear;
			public bool ignoreTimeScale = false;

			private void Awake()
			{
				gameObject.hideFlags = HideFlags.HideAndDontSave;
			}

			public void Move()
			{
				StartCoroutine(TranslateToRoutine());
			}

			private IEnumerator TranslateToRoutine()
			{
				Vector3 a, b;
				var elapsed = 0f;

				switch (relativeTo)
				{
					case Space.Self:
						a = target.localPosition;
						b = destination;

						while (elapsed < seconds)
						{
							var t = elapsed / seconds;
							t = easing.Interpolate(t);
							target.localPosition = Vector3.LerpUnclamped(a, b, t);

							yield return new WaitForEndOfFrame();

							elapsed += ignoreTimeScale
								? Time.deltaTime : Time.unscaledDeltaTime;
						}

						target.localPosition = b;

						break;

					case Space.World:
						a = target.position;
						b = destination;

						while (elapsed < seconds)
						{
							var t = elapsed / seconds;
							t = easing.Interpolate(t);
							target.position = Vector3.LerpUnclamped(a, b, t);

							yield return new WaitForEndOfFrame();

							elapsed += ignoreTimeScale
								? Time.deltaTime : Time.unscaledDeltaTime;
						}

						target.position = b;
						break;
				}

				DestroyImmediate(gameObject);
			}
		}
	}
}
