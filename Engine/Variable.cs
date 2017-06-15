using System;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[Serializable]
	public class Variable
	{
		[SerializeField]
		private string name;
		[SerializeField]
		private DynamicValue value;

		public string Name
		{
			get { return name; }
			set { name = value; }
		}

		public DynamicValue Value
		{
			get { return value; }
			set { this.value = value; }
		}

		public Variable() { }

		public Variable(string name, DynamicValue value)
		{
			this.name = name;
			this.value = value;
		}
	}
}