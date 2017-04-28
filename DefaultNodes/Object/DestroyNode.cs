using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Destroy", Path = "Object/Destroy")]
	public class DestroyNode : BakedNode
	{
		public enum LogType { Info, Warning, Error }

		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The object to destroy.")]
		[SerializeField, Input("To Destroy")]
		internal Object toDestroy = null;
		[Description("The number of seconds to wait before destroying the object.")]
		[SerializeField, Input("Seconds")]
		internal float seconds = 0;
		[Description("True if the object should be destroyed immediately.")]
		[SerializeField, Input("Immediate")]
		internal bool immediate = false;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			if (immediate)
			{
				DestroyImmediate(toDestroy, false);
			}
			else
			{
				Destroy(toDestroy, seconds);
			}

			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
