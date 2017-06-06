using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Pow", Path = "Math/Pow")]
	public class PowNode : BakedNode
	{
		[Description("A value.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The power.")]
		[SerializeField, Input("P")]
		internal float p = 1;

		[Description("The value of <i>F</i> raised to the power <i>P</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Pow(f, p);
		}

		#endregion
	}
}
