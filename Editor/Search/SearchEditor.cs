using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[Serializable]
	public class SearchEditor
	{
		[SerializeField]
		private bool isOpen = false;
		[SerializeField]
		private Vector2 position;
		[SerializeField]
		private Vector2 spawnPosition;
		[SerializeField]
		private Vector2 size;
		[SerializeField]
		private string searchStr = "";
		[SerializeField]
		private int selected = 0;
		[SerializeField]
		private NodeGraphPolicy policy;
		[SerializeField]
		private float scrollPos;

		private SyncList<SearchResult> results;
		private Job searchJob;

		private Texture2D highlight;
		private int resultCount;

		private bool justOpened = false;
		private Socket context;
		private bool contextWantsInput;
		private Type contextWantsType;

		bool searchNodes = true;

		#region Properties

		public bool IsOpen
		{
			get { return isOpen; }
			set { isOpen = value; }
		}

		public Vector2 Position
		{
			get { return position; }
			set { position = value; }
		}

		public Vector2 Size
		{
			get { return size; }
			set { size = value; }
		}

		#endregion

		private const int SEARCH_PADDING = 4;

		public SearchEditor(Vector2 size)
		{
			this.size = size;
		}

		public void Open
			(Vector2 position, Vector2 spawnPosition, NodeGraphPolicy policy,
			bool clear = false)
		{
			justOpened = true;
			isOpen = true;
			this.position = position;
			this.spawnPosition = spawnPosition;
			this.policy = policy;
			if (clear)
			{
				searchStr = "";
			}
		}

		public void Close()
		{
			isOpen = false;
		}

		public void SetWantedInputContext(GraphEditor editor, Socket socket)
		{
			context = socket;
			contextWantsInput = true;
			contextWantsType = socket.GetSocketType(editor.Graph);
		}

		public void SetWantedOutputContext(GraphEditor editor, Socket socket)
		{
			context = socket;
			contextWantsInput = false;
			contextWantsType = socket.GetSocketType(editor.Graph);
		}

		public void UnsetContext()
		{
			contextWantsType = null;
		}

		public void OnGUI(GraphEditor editor)
		{
			if (editor.Target != this)
			{
				Close();
				return;
			}
			if (!isOpen)
			{
				return;
			}

			GUI.enabled = true;

			var rect = new Rect();
			rect.size = size;
			rect.center = position + new Vector2(0, rect.size.y / 2);
			XGUI.ResetToStyle(GUI.skin.box);
			XGUI.Normal.background = GraphEditor.boxTexture;
			XGUI.Box(rect);

			rect.position += Vector2.one * SEARCH_PADDING;
			rect.size -= Vector2.one * SEARCH_PADDING * 2;
			XGUI.ResetToStyle(null);
			XGUI.BeginArea(rect);
			XGUI.BeginVertical();

			if (XEvent.IsKeyDown(KeyCode.Escape))
			{
				Close();
				XEvent.Use();
			}

			// Search bar
			XGUI.BeginHorizontal();
			XGUI.ResetToStyle(GUI.skin.button);
			var buttonString = searchNodes ? "Nodes" : "Variables";
			if (XGUI.Button(buttonString, XGUI.Width(70))) {
				searchNodes = !searchNodes;
			}

			bool keysUsed = false;
			var newSearchStr = searchStr;
			if (searchNodes)
			{
				// Detect key events before the text field
				switch (Event.current.type)
				{
					case EventType.KeyDown:
						switch (Event.current.keyCode)
						{
							case KeyCode.UpArrow:
								selected--;
								Event.current.Use();
								break;

							case KeyCode.DownArrow:
								selected++;
								Event.current.Use();
								break;

							case KeyCode.Home:
								selected = 0;
								Event.current.Use();
								break;

							case KeyCode.End:
								selected = int.MaxValue;
								Event.current.Use();
								break;

							case KeyCode.PageUp:
								selected -= 11;
								Event.current.Use();
								break;

							case KeyCode.PageDown:
								selected += 11;
								Event.current.Use();
								break;
						}

						keysUsed = Event.current.type == EventType.Used;
						break;
				}

				XGUI.ResetToStyle(GUI.skin.textField);
				GUI.SetNextControlName("search_field");
				newSearchStr = XGUI.TextField(searchStr);
				GUI.FocusControl("search_field");

				var countStr = "" + resultCount;
				if (searchJob != null && searchJob.IsRunning)
				{
					countStr += "...";
				}
				GUILayout.Label(countStr, GUILayout.ExpandWidth(false));
			}
			GUILayout.EndHorizontal();

			var hoveringOnResult = false;
			if (searchNodes)
			{
				hoveringOnResult = DrawNodeResults(editor, newSearchStr, keysUsed);
			}
			else
			{
				DrawVariableResults(editor);
			}

			GUILayout.EndVertical();
			GUILayout.EndArea();

			switch(Event.current.type)
			{
				case EventType.MouseDown:
					if (hoveringOnResult)
					{
						SelectResult(editor, selected);
						Close();
					}
					else if (rect.Contains(Event.current.mousePosition))
					{
						editor.Target = this;
						Event.current.Use();
					}
					break;

				case EventType.MouseUp:
					if (rect.Contains(Event.current.mousePosition))
					{
						editor.Target = this;
						Event.current.Use();
					}
					break;
			}
		}

		private Vector2 varScrollPos;

		private void DrawVariableResults(GraphEditor editor)
		{
			var variables = editor.Graph.Variables;

			if (variables.AsList().Count == 0)
			{
				XGUI.ResetToStyle(GUI.skin.label);
				XGUI.Alignment = TextAnchor.LowerCenter;
				XGUI.Enabled = false;
				XGUI.FontStyle = FontStyle.Italic;
				XGUI.Label("No variables", XGUI.MaxHeight(30));
			}
			else
			{
				varScrollPos = GUILayout.BeginScrollView(varScrollPos);
				foreach (var variable in variables)
				{
					XGUI.ResetToStyle(null);
					XGUI.BeginHorizontal();

					XGUI.ResetToStyle(GUI.skin.button);
					if (XGUI.Button("Get", XGUI.Width(40)))
					{
						var node = ScriptableObject.CreateInstance<DynamicNode>();
						node.name = "Get";

						node.AddOutputSocket(new DynamicSocket(
							variable.Value.Type, variable.Name));

						node.AddEvalInvoke(new EvalInvoke(
							variable.Name, variable.Name, variable.Name, InvokeType.GetVar));
						editor.AddNode(node, spawnPosition);
					}
					if (XGUI.Button("Set", XGUI.Width(40)))
					{
						var node = ScriptableObject.CreateInstance<DynamicNode>();
						node.name = "Set";

						node.AddInputSocket(new DynamicSocket(
							typeof(ExecType), "execIn"));
						node.AddInputSocket(new DynamicSocket(
							variable.Value.Type, "newValue"));

						node.AddOutputSocket(new DynamicSocket(
							typeof(ExecType), "execOut"));
						node.AddOutputSocket(new DynamicSocket(
							variable.Value.Type, variable.Name));

						node.AddExecInvoke(new ExecInvoke(
							"execIn", "execOut", "newValue", variable.Name, variable.Name, InvokeType.SetVar));
						editor.AddNode(node, spawnPosition);
					}

					XGUI.ResetToStyle(GUI.skin.label);
					XGUI.Label(variable.Name);

					XGUI.EndHorizontal();
				}

				GUILayout.EndScrollView();
			}
		}

		private bool DrawNodeResults
			(GraphEditor editor, string newSearchStr, bool keysUsed)
		{
			if (searchStr != newSearchStr || justOpened)
			{
				justOpened = false;

				if (searchJob != null)
				{
					searchJob.IsRunning = false;
				}

				// Define variables for capture
				var contextType = this.contextWantsType;
				var contextIsInput = this.contextWantsInput;
				var newResults = new SyncList<SearchResult>();
				results = newResults;

				// Perform search
				searchJob = new Job((job) =>
				{
					var scores = new List<int>();

					int timeout = 0;
					int n = 0;
					while (job.IsRunning)
					{
						// Check if the search items have changed (and restart
						// the search if so)
						if (n > policy.SearchItems.Count)
						{
							scores.Clear();
							newResults.Clear();
							n = 0;
						}

						// Get the next item, if available
						if (n == policy.SearchItems.Count)
						{
							if (timeout > 0)
							{
								break;
							}

							timeout++;
							Thread.Sleep(1000);
							continue;
						}

						timeout = 0;
						var item = policy.SearchItems[n++];

						if (contextType != null)
						{
							if (!item.MatchesContext(contextIsInput, contextType))
							{
								continue;
							}
						}

						// Score the item and insert it
						var score = FuzzySearch(newSearchStr, item.Label);
						if (score == int.MinValue)
						{
							continue;
						}

						int i = 0;
						for (; i < scores.Count; ++i)
						{
							if (score > scores[i] ||
								(score == scores[i] && item.Label.Length < newResults[i].Label.Length)
								)
							{
								scores.Insert(i, score);
								newResults.Insert(i, item);
								break;
							}
						}

						if (i == scores.Count)
						{
							scores.Add(score);
							newResults.Add(item);
						}
					}
				}).Start();
			}
			searchStr = newSearchStr;

			// Update the count
			if (Event.current.type != EventType.Repaint)
			{
				resultCount = results == null ? 0 : results.Count;
			}

			selected = Mathf.Clamp(selected, 0, resultCount - 1);
			if (keysUsed)
			{
				if (selected > Mathf.FloorToInt(scrollPos) + 11)
				{
					scrollPos = selected - 11;
				}
				else if (selected < scrollPos)
				{
					scrollPos = selected;
				}
			}
			scrollPos = Mathf.Clamp(scrollPos, 0, Mathf.Max(0, resultCount - 12));

			XGUI.ResetToStyle(null);
			XGUI.BeginHorizontal();
			XGUI.BeginVertical();
			// Show results
			int index = Mathf.Clamp((int)scrollPos, 0, resultCount);
			bool hoveringOnResult = false;
			for (int i = index; i < Mathf.Min(index + 12, resultCount); ++i)
			{
				var result = results[i];

				XGUI.ResetToStyle(GUI.skin.label);
				if (i == selected)
				{
					XGUI.Normal.background = GetHighlightTex();
				}
				XGUI.BeginHorizontal();

				XGUI.ResetToStyle(GUI.skin.label);
				if (i == selected)
				{
					XGUI.Normal.textColor = Color.white;
				}
				XGUI.RichText = true;
				XGUI.Alignment = TextAnchor.MiddleLeft;
				var score = "" + FuzzySearch(searchStr, result.Label);
				var highlight = FuzzyHighlight(searchStr, result.Label);
				XGUI.Label(score, XGUI.Width(30));
				XGUI.Label(highlight, XGUI.ExpandWidth(true));

				XGUI.EndHorizontal();

				var lastRect = GUILayoutUtility.GetLastRect();
				if (lastRect.Contains(Event.current.mousePosition))
				{
					if (Event.current.type == EventType.MouseMove)
					{
						selected = i;
						Event.current.Use();
					}
					hoveringOnResult = selected == i;
				}
			}
			// No results
			if (resultCount == 0)
			{
				XGUI.ResetToStyle(GUI.skin.label);
				XGUI.Alignment = TextAnchor.LowerCenter;
				XGUI.Enabled = false;
				XGUI.FontStyle = FontStyle.Italic;
				XGUI.Label("No results", XGUI.MaxHeight(30));
			}
			XGUI.EndVertical();
			scrollPos = GUILayout.VerticalScrollbar
				(scrollPos, Mathf.Min(resultCount, 12),
				0, Mathf.Max(resultCount, 12), GUILayout.ExpandHeight(true));
			XGUI.EndHorizontal();

			switch (Event.current.type)
			{
				case EventType.ScrollWheel:
					scrollPos += Event.current.delta.y;
					Event.current.Use();
					break;

				// Detect key events in the search field
				case EventType.Used:
					switch (Event.current.keyCode)
					{
						case KeyCode.KeypadEnter:
						case KeyCode.Return:
							SelectResult(editor, selected);
							Close();
							break;
					}
					break;
			}

			return hoveringOnResult;
		}

		private void SelectResult(GraphEditor editor, int index)
		{
			var node = results[index].MakeNode();
			editor.AddNode(node, spawnPosition);

			if (context != null)
			{
				// Attempt to connect the two nodes
				foreach (var other in node.GetSockets())
				{
					if (other.IsInput(editor.Graph) == contextWantsInput
						&& other.GetSocketType(editor.Graph) == contextWantsType)
					{
						editor.Graph.Links.Add(editor.Graph, context, other);
						break;
					}
				}
			}
		}

		private Texture2D GetHighlightTex()
		{
			if (!Util.IsNull(highlight))
			{
				return highlight;
			}

			Color[] pixels = new Color[1];
			pixels[0] = new Color32(61, 128, 223, 255);

			highlight = new Texture2D(1, 1);
			highlight.SetPixels(pixels);
			highlight.Apply();

			return highlight;
		}

		private string FuzzyHighlight(string pattern, string str)
		{
			string ret = "";
			int pi = 0, si = 0;
			while (pi < pattern.Length && si < str.Length)
			{
				if (char.ToLower(pattern[pi]) == char.ToLower(str[si]))
				{
					ret += "<b>" + str[si] + "</b>";
					pi++;
				}
				else
				{
					ret += str[si];
				}
				si++;
			}

			if (si != str.Length)
			{
				ret += str.Substring(si);
			}

			return ret;
		}

		private int FuzzySearch(string pattern, string str,
			int patternStart = 0, int stringStart = 0)
		{
			bool start = true;
			bool prev = false;
			int bestScore = int.MinValue, score = 0;
			int pi = patternStart, si = stringStart;
			while (pi < pattern.Length && si < str.Length)
			{
				// Increase score
				if (char.ToLower(pattern[pi]) == char.ToLower(str[si]))
				{
					// Exhaustive search
					var skipScore = FuzzySearch(pattern, str, pi, si + 1);
					if (skipScore != int.MinValue)
					{
						bestScore = Mathf.Max(bestScore, score + skipScore);
					}

					// Starting letter
					if (start)
					{
						score += 15;
					}
					// Consecutive letters
					else if (prev)
					{
						score += 15;
					}
					// Uppercase letters
					else if (!prev && char.IsUpper(str[si]))
					{
						score += 10;
					}
					// Seperator bonus
					else if ('.' == pattern[pi] || '/' == pattern[pi])
					{
						score += 10;
					}

					prev = true;
					pi++;
				}
				// Decrease score
				else
				{
					// Unmatched letter
					score -= 2;
					prev = false;
				}

				start = (str[si] == '.' || str[si] == '/');
				si++;
			}

			// Check if the pattern matched
			if (pi != pattern.Length)
			{
				return int.MinValue;
			}

			// Score remaining unmatched letters
			score -= (str.Length - si) * 2;

			return Mathf.Max(score, bestScore);
		}
	}
}
