using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Inverse Lerp", Path = "Math/Inverse Lerp")]
	public class InverseLerpNode : BakedNode
	{
		[Description("A value.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The start value.")]
		[SerializeField, Input("A")]
		internal float a = 0;

		[Description("The end value.")]
		[SerializeField, Input("B")]
		internal float b = 1;

		[Description("The linear parameter <i>T</i> that produces the interpolant <i>F</i> within the range <i>[A, B]</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.InverseLerp(a, b, f);
		}

		#endregion
	}
}
