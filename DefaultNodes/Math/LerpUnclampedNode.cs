using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Lerp Unclamped", Path = "Math/Lerp Unclamped")]
	public class LerpUnclampedNode : BakedNode
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

		[Description("The unclamped interpolated float result.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.LerpUnclamped(a, b, t);
		}

		#endregion
	}
}
