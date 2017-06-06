using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Abs", Path = "Math/Abs")]
	public class AbsNode : BakedNode
	{
		[Description("A value.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The absolute value of <i>F</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Abs(f);
		}

		#endregion
	}
}
