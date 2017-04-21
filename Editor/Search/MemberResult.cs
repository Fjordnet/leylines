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
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	public class MemberResult : SearchResult
	{
		public override string Label
		{
			get
			{
				string suffix;

				switch (Member.MemberType) {
					case MemberTypes.Method:
						suffix = "()";
						break;
					case MemberTypes.Field:
					case MemberTypes.Property:
						suffix = "";
						break;
					default:
						suffix = "?";
						break;
				}

				return string.Format("{0}.{1}{2}",
					Type.FullName.Replace('.', '/'),
					Member.Name,
					suffix
					);
			}
		}

		public readonly Type Type;
		public readonly MemberInfo Member;

		public MemberResult(Type type, MemberInfo member)
		{
			Type = type;
			Member = member;
		}

		public override void Pick(Graph graph, Vector2 spawnPosition)
		{
			var node = CreateNode(Type.Name, spawnPosition);

			Undo.RegisterCreatedObjectUndo(node, null);

			AddNode(graph, node);

			switch (Member.MemberType) {
					case MemberTypes.Method:
						node.name += "." + Member.Name + "()";

						var method = (MethodInfo)Member;
						node.AddInputSocket(
							new DynamicSocket(typeof(ExecType), "execIn", false, true));
						node.AddInputSocket(
							new DynamicSocket(Type, Type.Name, true, false));

						var @params = new List<string>();
						foreach (var param in method.GetParameters()) {
							node.AddInputSocket(
								new DynamicSocket(param.ParameterType, param.Name, true, false));
							@params.Add(param.Name);
						}

						node.AddOutputSocket(
							new DynamicSocket(typeof(ExecType), "execOut", false, true));
						if (method.ReturnType != typeof(void))
						{
							node.AddOutputSocket(
								new DynamicSocket(method.ReturnType, "result", false, true));
						}

						node.AddExecInvoke(
							new ExecInvoke("execIn", "execOut", Type.Name, "result", method.Name, InvokeType.CallMethod, @params));

						break;

					case MemberTypes.Field:
						node.name += "." + Member.Name;

						var field = (FieldInfo)Member;
						node.AddInputSocket(
							new DynamicSocket(Type, Type.Name, true, false));

						node.AddOutputSocket(
							new DynamicSocket(field.FieldType, Member.Name, false, true));

						node.AddEvalInvoke(
							new EvalInvoke(Type.Name, "result", Member.Name, InvokeType.GetProperty));
						break;

					case MemberTypes.Property:
						node.name += "." + Member.Name;

						var property = (PropertyInfo)Member;
						node.AddInputSocket(
							new DynamicSocket(Type, Type.Name, true, true));

						node.AddOutputSocket(
							new DynamicSocket(property.PropertyType, Member.Name, false, true));

						node.AddEvalInvoke(
							new EvalInvoke(Type.Name, "result", Member.Name, InvokeType.GetProperty));
						break;

					default:
						Debug.LogError(string.Format(
							"MemberType {0} is not supported.",
							Member.MemberType));
						break;
				}
		}
	}
}