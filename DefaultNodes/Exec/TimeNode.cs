using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Time", Path = "Exec/Time")]
	public class TimeNode : BakedNode
	{
		public enum TimeType
		{
			DeltaTime, UnscaledDeltaTime, SmoothDeltaTime,
			SecondsSinceGameStart, UnscaledSecondsSinceGameStart,
		}

		[SerializeField, Input("Type")]
		internal TimeType type = TimeType.DeltaTime;

		[SerializeField, Output("Delta")]
		internal float delta = 0;

		#region Methods

		public override void Eval(GraphScope scope)
		{
			switch(type)
			{
				case TimeType.DeltaTime:
					delta = Time.deltaTime;
					break;

				case TimeType.UnscaledDeltaTime:
					delta = Time.unscaledDeltaTime;
					break;

				case TimeType.SmoothDeltaTime:
					delta = Time.smoothDeltaTime;
					break;

				case TimeType.SecondsSinceGameStart:
					delta = Time.time;
					break;

				case TimeType.UnscaledSecondsSinceGameStart:
					delta = Time.unscaledTime;
					break;
			}
		}

		#endregion
	}
}
