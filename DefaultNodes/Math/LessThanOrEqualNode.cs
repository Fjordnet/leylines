using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "<=", Path = "Math/Less Than  Or Equal (<=)")]
	public class LessThanOrEqualNode : BakedNode
	{
		[Description("The left-hand value.")]
		[SerializeField, Input("L")]
		internal float l = 0;

		[Description("The right-hand value.")]
		[SerializeField, Input("R")]
		internal float r = 0;

		[Description("The result of <i>L < R</i>.")]
		[SerializeField, Output("Result")]
		internal bool result = false;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = l <= r;
		}

		#endregion
	}
}
