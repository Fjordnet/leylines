using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Spawn", Path = "Object/Spawn")]
	public class SpawnNode : BakedNode
	{
		public enum LogType { Info, Warning, Error }

		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The object to spawn.")]
		[SerializeField, Input("Spawn")]
		internal Object spawn = null;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;
		[Description("The object that was spawned.")]
		[SerializeField, Output("Spawned")]
		internal Object spawned = null;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			spawned = Instantiate(spawn);
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
