using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Lerp", Path = "Math/Lerp")]
	public class LerpNode : BakedNode
	{
		[Description("The interpolation value between the two floats.")]
		[SerializeField, Input("T")]
		internal float t = 0;

		[Description("The start value.")]
		[SerializeField, Input("A")]
		internal float a = 0;

		[Description("The end value.")]
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
			result = Mathf.Lerp(a, b, t);
		}

		#endregion
	}
}
