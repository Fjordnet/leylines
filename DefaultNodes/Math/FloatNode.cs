using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Float", Path = "Math/Float")]
	public class FloatNode : BakedNode
	{
		[Description("The value.")]
		[SerializeField, Output("Value", SocketFlags.Editable | SocketFlags.AllowMultipleLinks)]
		internal float value = 0;

		public override float OutputWidth
		{
			get { return 40; }
		}
	}
}
