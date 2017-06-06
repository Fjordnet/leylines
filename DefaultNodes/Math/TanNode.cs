using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Tan", Path = "Math/Tan")]
	public class TanNode : BakedNode
	{
		[Description("A value in radians.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The tangent of angle <i>F</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Tan(f);
		}

		#endregion
	}
}
