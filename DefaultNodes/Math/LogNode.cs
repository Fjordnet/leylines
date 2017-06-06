using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Log", Path = "Math/Log")]
	public class LogNode : BakedNode
	{
		[Description("A number.")]
		[SerializeField, Input("F")]
		internal float f = 0;

		[Description("The base value.")]
		[SerializeField, Input("P")]
		internal float p = (float)System.Math.E;

		[Description("The logarithm of the number <i>F</i> in a specified base <i>P</i>.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.Log(f, p);
		}

		#endregion
	}
}
