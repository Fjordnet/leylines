using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Atan2", Path = "Math/Atan2")]
	public class Atan2Node : BakedNode
	{
		[Description("A value.")]
		[SerializeField, Input("Y")]
		internal float y = 0;

		[Description("A value.")]
		[SerializeField, Input("X")]
		internal float x = 0;

		[Description("The angle in radians whose tangent is <i>Y / X</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Atan2(y, x);
		}

		#endregion
	}
}
