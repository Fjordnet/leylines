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
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// Stores a reference to a graph asset and a player for it.
	/// </summary>
	public class NodeGraph : MonoBehaviour
	{
		public Graph Graph
		{
			get { return graph; }
			set { graph = value; }
		}
		[SerializeField]
		private Graph graph = null;

		public NodeGraphPlayer Player
		{
			get
			{
				if (Util.IsNull(player))
				{
					GameObject go = new GameObject("Node Graph Player");
					go.hideFlags = HideFlags.HideInHierarchy
						| HideFlags.DontSaveInEditor
						| HideFlags.DontSaveInBuild;

					player = go.AddComponent<NodeGraphPlayer>();
					player.NodeGraph = this;
				}
				return player;
			}
		}
		private NodeGraphPlayer player;

		#region MonoBehaviour

		void Awake()
		{
			Player.Exec(ExecType.OnAwake);
		}

		void Start()
		{
			Player.Exec(ExecType.OnStart);
		}

		void OnDestroy()
		{
			Player.Exec(ExecType.OnDestroy);
		}

		void OnEnable()
		{
			Player.Exec(ExecType.OnEnable);
		}

		void OnDisable()
		{
			Player.Exec(ExecType.OnDisable);
		}

		void Update()
		{
			Player.Exec(ExecType.OnUpdate);
		}

		void FixedUpdate()
		{
			Player.Exec(ExecType.OnFixedUpdate);
		}

		void LateUpdate()
		{
			Player.Exec(ExecType.OnLateUpdate);
		}

		void OnApplicationFocus()
		{
			Player.Exec(ExecType.OnApplicationFocus);
		}

		void OnApplicationPause()
		{
			Player.Exec(ExecType.OnApplicationPause);
		}

		void OnApplicationQuit()
		{
			Player.Exec(ExecType.OnApplicationQuit);
		}

		#endregion
	}
}
