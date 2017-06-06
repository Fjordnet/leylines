using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Atan", Path = "Math/Atan")]
	public class AtanNode : BakedNode
	{
		[Description("A value in radians.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The angle in radians whose tangent is <i>F</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Atan(f);
		}

		#endregion
	}
}
