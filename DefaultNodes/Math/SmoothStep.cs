using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Smooth Step", Path = "Math/Smooth Step")]
	public class SmoothStepNode : BakedNode
	{
		[Description("The start value.")]
		[SerializeField, Input("A")]
		internal float a = 0;

		[Description("The end value.")]
		[SerializeField, Input("B")]
		internal float b = 1;

		[Description("The interpolation value between the two floats.")]
		[SerializeField, Input("T")]
		internal float t = 0;

		[Description("The interpolated value between [A, B] with smoothing at the limits.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.SmoothStep(a, b, t);
		}

		#endregion
	}
}
