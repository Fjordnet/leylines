using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Lerp Angle", Path = "Math/Lerp Angle")]
	public class LerpAngleNode : BakedNode
	{
		[Description("The interpolation value between the two angles.")]
		[SerializeField, Input("T")]
		internal float t = 0;

		[Description("The start value in degrees.")]
		[SerializeField, Input("A")]
		internal float a = 0;

		[Description("The end value in degrees.")]
		[SerializeField, Input("B")]
		internal float b = 1;

		[Description("The clamped interpolated float result between <i>[A, B]</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.LerpAngle(a, b, t);
		}

		#endregion
	}
}
