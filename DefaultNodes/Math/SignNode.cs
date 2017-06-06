using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Sign", Path = "Math/Sign")]
	public class SignNode : BakedNode
	{
		[Description("A value.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The sign of <i>F</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Sign(f);
		}

		#endregion
	}
}
