using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Find Child", Path = "Object/Find Child")]
	public class FindChildNode : BakedNode
	{
		[Description("The Transform to search in.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("The name of the child to search for.")]
		[SerializeField, Input("Name")]
		internal string childName = "";

		[Description("The GameObject, if found.")]
		[SerializeField, Output("GameObject")]
		internal GameObject childGameObject = null;
		[Description("The Transform, if found.")]
		[SerializeField, Output("Transform")]
		internal Transform childTransform = null;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			if (!Util.IsNull(transform))
			{
				childTransform = transform.Find(childName);
			}
			else
			{
				childTransform = null;
			}

			if (!Util.IsNull(childTransform))
			{
				childGameObject = childTransform.gameObject;
			}
			else
			{
				childGameObject = null;
			}

			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
