﻿using System.Collections.Generic;
using UnityEngine;

namespace Exodrifter.NodeGraph.DefaultNodes
{
	[Node(Name = "Translate", Path = "Transform/Translate")]
	public class TranslateNode : BakedNode
	{
		[Description("The input signal.")]
		[SerializeField, Input("Exec", (SocketFlags)0)]
		internal ExecType execIn = ExecType.None;
		[Description("The transform to translate.")]
		[SerializeField, Input("Transform")]
		internal Transform transform = null;
		[Description("Defines the translation in local or world space.")]
		[SerializeField, Input("Relative To")]
		internal Transform relativeTo = null;
		[Description("The number of meters to translate by.")]
		[SerializeField, Input("Meters")]
		internal Vector3 meters = Vector3.zero;
		[Description("The delta to scale the degrees by.")]
		[SerializeField, Input("Delta")]
		internal float delta = 1;

		[Description("The output signal.")]
		[SerializeField, Output("Exec")]
		internal ExecType execOut = ExecType.None;
		[Description("The transform used, for chaining.")]
		[SerializeField, Output("Transform")]
		internal Transform transformOut = null;
		[Description("The delta used, for chaining.")]
		[SerializeField, Output("Delta")]
		internal float deltaOut = 1;

		#region Methods

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			transform.Translate(meters * delta, relativeTo);
			transformOut = transform;
			deltaOut = delta;
			yield return new SignalSocket(scope, GetSocket("execOut"));
		}

		#endregion
	}
}
