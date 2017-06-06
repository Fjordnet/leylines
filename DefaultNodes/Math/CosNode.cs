using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Cos", Path = "Math/Cos")]
	public class CosineNode : BakedNode
	{
		[Description("A value in radians.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The cosine of angle <i>F</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Cos(f);
		}

		#endregion
	}
}
