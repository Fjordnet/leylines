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
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[Serializable]
	public class ExecInvoke : IInvoke
	{
		[SerializeField]
		private string trigger;
		[SerializeField]
		private string next;
		[SerializeField]
		private string target;
		[SerializeField]
		private string result;
		[SerializeField]
		private string memberName;
		[SerializeField]
		private InvokeType invokeType;
		[SerializeField]
		private List<string> args;

		#region Properties

		/// <summary>
		/// The socket that triggers this invoke.
		/// </summary>
		public string Trigger
		{
			get { return trigger; }
			set { trigger = value; }
		}

		/// <summary>
		/// The socket execution will continue on.
		/// </summary>
		public string Next
		{
			get { return next; }
			set { next = value; }
		}

		/// <summary>
		/// The field name of the socket to invoke on.
		/// </summary>
		public string Target
		{
			get { return target; }
			set { target = value; }
		}

		/// <summary>
		/// The field name of the socket to store the result.
		/// </summary>
		public string Result
		{
			get { return result; }
			set { result = value; }
		}

		/// <summary>
		/// The name of the member to invoke.
		/// </summary>
		public string MemberName
		{
			get { return memberName; }
			set { memberName = value; }
		}

		/// <summary>
		/// The type of invoke to perform.
		/// </summary>
		public InvokeType InvokeType
		{
			get { return invokeType; }
			set { invokeType = value; }
		}

		/// <summary>
		/// The list of socket field names to use as arguments.
		/// </summary>
		public List<string> Args
		{
			get { return args; }
			set { args = value; }
		}

		#endregion

		public ExecInvoke(string trigger, string next,
			string target, string result, string memberName,
			InvokeType invokeType, List<string> args)
		{
			this.trigger = trigger;
			this.next = next;
			this.target = target;
			this.result = result;
			this.memberName = memberName;
			this.invokeType = invokeType;
			this.args = args;
		}

		public ExecInvoke(string trigger, string next,
			string target, string result, string memberName,
			InvokeType invokeType, params string[] args)
		{
			this.trigger = trigger;
			this.next = next;
			this.target = target;
			this.result = result;
			this.memberName = memberName;
			this.invokeType = invokeType;
			this.args = new List<string>(args);
		}
	}
}
