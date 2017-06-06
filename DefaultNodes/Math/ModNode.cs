using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Mod", Path = "Math/Mod")]
	public class ModNode : BakedNode
	{
		[Description("The left-hand value.")]
		[SerializeField, Input("L")]
		internal float l = 0;

		[Description("The right-hand value.")]
		[SerializeField, Input("R")]
		internal float r = 0;

		[Description("The result of <i>L % R</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = l % r;
		}

		#endregion
	}
}
