using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Asin", Path = "Math/Asin")]
	public class AsinNode : BakedNode
	{
		[Description("A value in radians.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The angle in radians whose sine is <i>F</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Asin(f);
		}

		#endregion
	}
}
