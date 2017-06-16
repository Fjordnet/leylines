/* Unity3D Node Graph Framework
Copyright (c) 2017 Ava Pek

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	public class DynamicNode : Node
	{
		[SerializeField]
		private float inputWidth = INPUT_WIDTH;
		[SerializeField]
		private float outputWidth = OUTPUT_WIDTH;
		[SerializeField]
		private List<ExecInvoke> execInvokes;
		[SerializeField]
		private List<EvalInvoke> evalInvokes;
		[SerializeField]
		private List<DynamicSocket> inputSockets;
		[SerializeField]
		private List<DynamicSocket> outputSockets;

		#region Properties

		public override string DisplayName
		{
			get { return name; }
			set { name = value; }
		}

		public override float InputWidth { get { return inputWidth; } }
		public override float OutputWidth { get { return outputWidth; } }

		#endregion

		#region Node Logic

		public override IEnumerator<Yield> Exec
			(Socket from, Socket to, GraphScope scope)
		{
			foreach (var invoke in execInvokes)
			{
				if (invoke.Trigger != to.FieldName)
				{
					continue;
				}

				Invoke(scope, invoke);

				var next = GetSocket(invoke.Next);
				if (!Util.IsNull(next))
				{
					yield return new SignalSocket(scope, next);
				}

				break;
			}
		}

		public override void Eval(GraphScope scope)
		{
			foreach (var invoke in evalInvokes)
			{
				Invoke(scope, invoke);
			}
		}

		private void Invoke(GraphScope scope, IInvoke invoke)
		{
			var targetSocket = GetSocket(invoke.Target);
			if (Util.IsNull(targetSocket))
			{
				return;
			}

			var type = GetSocketType(targetSocket);
			var target = GetSocketValue(targetSocket);
			var flags = BindingFlags.Instance | BindingFlags.NonPublic
				| BindingFlags.FlattenHierarchy | BindingFlags.Public;

			object value;
			object[] args;
			switch (invoke.InvokeType)
			{
				case InvokeType.GetField:
					var gField = type.GetField(invoke.MemberName, flags);
					value = gField.GetValue(target);
					SetSocketValue(invoke.Result, value);
					break;

				case InvokeType.SetField:
					var sField = type.GetField(invoke.MemberName, flags);
					value = GetSocketValue(invoke.Args[0]);
					sField.SetValue(target, value);
					break;

				case InvokeType.GetProperty:
					var gProperty = type.GetProperty(invoke.MemberName, flags);
					args = BuildArgs(0, invoke.Args);
					value = gProperty.GetValue(target, args);
					SetSocketValue(invoke.Result, value);
					break;

				case InvokeType.SetProperty:
					var sProperty = type.GetProperty(invoke.MemberName, flags);
					args = BuildArgs(1, invoke.Args);
					value = GetSocketValue(invoke.Args[0]);
					sProperty.SetValue(target, value, args);
					break;

				case InvokeType.CallMethod:
					var argTypes = BuildArgTypes(0, invoke.Args);
					var method = type.GetMethod(invoke.MemberName, flags, null, argTypes, null);
					args = BuildArgs(0, invoke.Args);
					value = method.Invoke(target, args);
					if (method.ReturnType != typeof(void)) {
						SetSocketValue(invoke.Result, value);
					}
					break;

				case InvokeType.SetVar:
					value = GetSocketValue(invoke.Target);
					scope.Vars[invoke.MemberName].Value = value;
					SetSocketValue(invoke.Result, value);
					break;

				case InvokeType.GetVar:
					Debug.Log("Getting " + invoke.Target);
					value = scope.Vars[invoke.Target].Value;
					SetSocketValue(invoke.Result, value);
					break;

				default:
					var msg = string.Format(
						"Invoke type {0} not supported!",
						invoke.InvokeType);
					throw new InvalidOperationException(msg);
			}
		}

		public void AddExecInvoke(ExecInvoke invoke)
		{
			if (execInvokes == null)
			{
				execInvokes = new List<ExecInvoke>();
			}

			execInvokes.Add(invoke);
		}

		public void AddEvalInvoke(EvalInvoke invoke)
		{
			if (evalInvokes == null)
			{
				evalInvokes = new List<EvalInvoke>();
			}

			evalInvokes.Add(invoke);
		}

		#endregion

		#region Socket Getters and Setters

		public void AddInputSocket(DynamicSocket definition)
		{
			if (inputSockets == null)
			{
				inputSockets = new List<DynamicSocket>();
			}

			inputSockets.Add(definition);
		}

		public void AddOutputSocket(DynamicSocket definition)
		{
			if (outputSockets == null)
			{
				outputSockets = new List<DynamicSocket>();
			}

			outputSockets.Add(definition);
		}

		public override Socket[] GetInputSockets()
		{
			if (inputSockets == null)
			{
				return new Socket[] { };
			}

			return inputSockets.Select(
				(def) =>
				{
					return new Socket(ID, def.SocketName);
				}
			).ToArray();
		}

		public override Socket[] GetOutputSockets()
		{
			if (outputSockets == null)
			{
				return new Socket[] { };
			}

			return outputSockets.Select(
				(def) =>
				{
					return new Socket(ID, def.SocketName);
				}
			).ToArray();
		}

		public override Socket GetSocket(string name)
		{
			var def = GetDynamicSocket(name);
			return new Socket(ID, def.SocketName);
		}

		public override Socket[] GetSockets()
		{
			var ret = new List<Socket>();
			ret.AddRange(GetInputSockets());
			ret.AddRange(GetOutputSockets());
			return ret.ToArray();
		}

		#endregion

		#region Socket Properties

		public override string GetSocketDescription(string name)
		{
			var socket = GetDynamicSocket(name);
			return socket.Description;
		}

		public override string GetSocketDisplayName(string name)
		{
			var socket = GetDynamicSocket(name);
			return socket.SocketName;
		}

		public override SocketFlags GetSocketFlags(string name)
		{
			var socket = GetDynamicSocket(name);
			return socket.Flags;
		}

		public override Type GetSocketType(string name)
		{
			var socket = GetDynamicSocket(name);
			return socket.SocketType;
		}

		public override object GetSocketValue(string name)
		{
			var socket = GetDynamicSocket(name);
			return socket.SocketValue;
		}

		public override bool IsSocketInput(string name)
		{
			if (inputSockets != null)
			{
				foreach (var socket in inputSockets)
				{
					if (Util.IsNull(socket))
					{
						continue;
					}
					if (socket.SocketName == name)
					{
						return true;
					}
				}
			}
			if (outputSockets != null)
			{
				foreach (var socket in outputSockets)
				{
					if (Util.IsNull(socket))
					{
						continue;
					}
					if (socket.SocketName == name)
					{
						return false;
					}
				}
			}
			return false;
		}

		public override void SetSocketValue(string name, object value)
		{
			var socket = GetDynamicSocket(name);
			socket.SocketValue = value;
		}

		#endregion

		#region Util

		/// <summary>
		/// Builds an object argument array from a list of sockets.
		/// </summary>
		/// <param name="startIndex">The index to start building at.</param>
		/// <param name="socketArgs">The sockets to use as arguments.</param>
		/// <returns>The object argument array.</returns>
		private object[] BuildArgs(int startIndex, List<string> socketArgs)
		{
			var length = socketArgs.Count - startIndex;

			if (length <= 0)
			{
				return null;
			}
			var ret = new object[length];

			for (int i = 0; i < length; ++i)
			{
				ret[i] = GetSocketValue(socketArgs[startIndex + i]);
			}

			return ret;
		}

		/// <summary>
		/// Builds an object type argument array from a list of sockets.
		/// </summary>
		/// <param name="startIndex">The index to start building at.</param>
		/// <param name="socketArgs">The sockets to use as arguments.</param>
		/// <returns>The object argument array.</returns>
		private Type[] BuildArgTypes(int startIndex, List<string> socketArgs)
		{
			var length = socketArgs.Count - startIndex;
			var ret = new Type[length];

			for (int i = 0; i < length; ++i)
			{
				var socket = GetDynamicSocket(socketArgs[startIndex + i]);
				ret[i] = socket.SocketType;
			}

			return ret;
		}

		private DynamicSocket GetDynamicSocket(string name)
		{
			DynamicSocket ret = null;

			if (inputSockets != null)
			{
				ret = inputSockets
					.Where((d) => d.SocketName == name).FirstOrDefault();
			}
			if (ret == null && outputSockets != null)
			{
				ret = outputSockets
					.Where((d) => d.SocketName == name).FirstOrDefault();
			}

			return ret;
		}

		#endregion
	}
}
