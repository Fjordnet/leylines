using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Log", Path = "Debug/Log")]
	public class DebugLogNode : BakedNode
	{
		public enum LogType { Info, Warning, Error }

		[Description("The input signal.")]
		[SerializeField, Input("Exec", 0)]
		internal ExecType execIn = ExecType.None;
		[Description("The severity of the log message.")]
		[SerializeField, Input("Severity")]
		internal LogType severity = LogType.Info;
		[Description("The context to use for the message.")]
		[SerializeField, Input("Context")]
		internal Object context = null;
		[Description("The message to log.")]
		[SerializeField, Input("Message")]
		internal string message = "";

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			switch(severity)
			{
				case LogType.Info:
					Debug.Log(message, context);
					break;
				case LogType.Warning:
					Debug.LogWarning(message, context);
					break;
				case LogType.Error:
					Debug.LogError(message, context);
					break;
			}

			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
