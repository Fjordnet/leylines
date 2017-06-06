using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Equal", Path = "Math/Equal")]
	public class EqualNode : BakedNode
	{
		[Description("The left-hand value.")]
		[SerializeField, Input("L")]
		internal float l = 0;

		[Description("The right-hand value.")]
		[SerializeField, Input("R")]
		internal float r = 0;

		[Description("True if L and R are similar.")]
		[SerializeField, Output("Result")]
		internal bool result = false;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Approximately(l, r);
		}

		#endregion
	}
}
