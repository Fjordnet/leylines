using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	/// <summary>
	/// Returns a reference to the GameObject this node graph is on.
	/// </summary>
	[Node(Name = "Self", Path = "Object/Self")]
	public class SelfNode : BakedNode
	{
		[Description("The GameObject this node graph is on.")]
		[SerializeField, Output("GameObject")]
		internal GameObject gameObject = null;
		[Description("The Transform this node graph is on.")]
		[SerializeField, Output("Transform")]
		internal Transform transform = null;

		#region Methods

		public override void Eval(GraphScope scope)
		{
			gameObject = scope.GameObject;
			transform = gameObject.transform;
		}

		#endregion
	}
}
