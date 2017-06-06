using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Perlin Noise", Path = "Math/Perlin Noise")]
	public class PerlinNoiseNode : BakedNode
	{
		[Description("The x-coordinate sample point.")]
		[SerializeField, Input("X")]
		internal float x = 0;

		[Description("The y-coordinate sample point.")]
		[SerializeField, Input("Y")]
		internal float y = 1;

		[Description("The generated 2D Perlin noise value.")]
		[SerializeField, Output("Result")]
		internal float result = 0;

		public override float InputWidth
		{
			get { return 40; }
		}

		#region Methods

		public override void Eval(GraphScope scope)
		{
			result = Mathf.PerlinNoise(x, y);
		}

		#endregion
	}
}
