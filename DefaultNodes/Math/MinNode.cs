using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Min", Path = "Math/Min")]
	public class MinNode : BakedNode
	{
		[Description("A value.")]
		[SerializeField, Input("A")]
		internal float a = 0;

		[Description("A value.")]
		[SerializeField, Input("B")]
		internal float b = 1;

		[Description("The smallest of values <i>A</i> and <i>B</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Min(a, b);
		}

		#endregion
	}
}
