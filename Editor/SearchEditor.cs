using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
			(Vector2 position, Vector2 spawnPosition, NodeGraphPolicy policy)
		{
			isOpen = true;
			this.position = position;
			this.spawnPosition = spawnPosition;
			this.policy = policy;
		}

		public void Close()
		{
			isOpen = false;
		}

		public void OnGUI(GraphEditor editor)
		{
			if (editor.Target != this)
			{
				Close();
				return;
			}
			if (!isOpen) {
				return;
			}

			var rect = new Rect();
			rect.size = size;
			rect.center = position + new Vector2(0, rect.size.y / 2);

			GUI.Box(rect, GUIContent.none);

			rect.position += Vector2.one * SEARCH_PADDING;
			rect.size -= Vector2.one * SEARCH_PADDING * 2;
			GUILayout.BeginArea(rect);
			GUILayout.BeginVertical();

			// Detect key events before the text field
			var keysUsed = false;
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

			GUILayout.BeginHorizontal();
			GUILayout.Label("Search:", GUILayout.ExpandWidth(false));
			GUI.SetNextControlName("search_field");
			var newSearchStr = GUILayout.TextField(searchStr);
			GUI.FocusControl("search_field");

			if (searchStr != newSearchStr)
			{
				if (searchJob != null)
				{
					searchJob.IsRunning = false;
				}

				var newResults = new SyncList<SearchResult>();
				results = newResults;
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

						// Score the item and insert it
						var score = FuzzySearch(newSearchStr, item.Label);
						if (score == int.MinValue)
						{
							continue;
						}

						int i = 0;
						for (; i < scores.Count; ++i)
						{
							if (score > scores[i])
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

			var countStr = "" + resultCount;
			if (searchJob != null && searchJob.IsRunning)
			{
				countStr += "...";
			}
			GUILayout.Label(countStr, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();

			var oldRichText = GUI.skin.label.richText;
			GUI.skin.label.richText = true;

			GUILayout.BeginHorizontal();
			var scrollSize = GUI.skin.verticalScrollbar.fixedWidth;
			GUILayout.BeginVertical();
			// Show results
			int index = Mathf.Clamp((int)scrollPos, 0, resultCount);
			bool hoveringOnResult = false;
			for (int i = index; i < Mathf.Min(index + 12, resultCount); ++i)
			{
				var result = results[i];

				GUIStyle style = new GUIStyle(GUI.skin.label);
				var oldAlignment = GUI.skin.label.alignment;
				var oldColor = GUI.skin.label.normal.textColor;
				GUI.skin.label.alignment = TextAnchor.MiddleLeft;
				if (i == selected)
				{
					style.normal.background = GetHighlightTex();

					GUILayout.BeginHorizontal(style);
					GUI.skin.label.normal.textColor = Color.white;
				}
				else
				{
					GUILayout.BeginHorizontal(style);
				}

				var label = FuzzySearch(searchStr, result.Label) + " "
					+ FuzzyHighlight(searchStr, result.Label);
				GUILayout.Label(label, GUILayout.Width(rect.width - scrollSize
					- style.padding.left * 3 - style.padding.right * 3));
				GUILayout.EndHorizontal();

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

				GUI.skin.label.normal.textColor = oldColor;
				GUI.skin.label.alignment = oldAlignment;
			}
			// No results
			if (resultCount == 0) {
				var oldAlignment = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.LowerCenter;
				GUI.enabled = false;
				GUILayout.Label("<i>No results</i>", GUILayout.MaxHeight(30));
				GUI.enabled = true;
				GUI.skin.label.alignment = oldAlignment;
			}
			GUILayout.EndVertical();
			scrollPos = GUILayout.VerticalScrollbar
				(scrollPos, Mathf.Min(resultCount, 12),
				0, Mathf.Max(resultCount, 12), GUILayout.ExpandHeight(true));
			GUILayout.EndHorizontal();

			GUI.skin.label.richText = oldRichText;

			GUILayout.EndVertical();
			GUILayout.EndArea();

			switch (Event.current.type)
			{
				case EventType.MouseDown:
					if (hoveringOnResult)
					{
						var node = results[selected].MakeNode();
						editor.AddNode(node, spawnPosition);
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

				case EventType.ScrollWheel:
					scrollPos += Event.current.delta.y;
					Event.current.Use();
					break;

				// Detect key events in the search field
				case EventType.Used:
					switch(Event.current.keyCode)
					{
						case KeyCode.KeypadEnter:
						case KeyCode.Return:
							var node = results[selected].MakeNode();
							editor.AddNode(node, spawnPosition);
							Close();
							break;

						case KeyCode.Escape:
							Close();
							break;
					}
					break;
			}
		}

		private Texture2D GetHighlightTex()
		{
			if (!Util.IsNull(highlight)) {
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
					bestScore = Mathf.Max(bestScore, skipScore);

					// Uppercase letters
					if (char.IsUpper(str[si])) {
						score += 20;
					}
					// Seperator bonus
					if ('.' == pattern[pi] || '/' == pattern[pi]) {
						score += 20;
					}
					// Consecutive letters
					if (prev) {
						score += 5;
					}

					prev = true;
					pi++;
				}
				// Decrease score
				else
				{
					// Unmatched letter
					score -= 1;

					prev = false;
				}
				si++;
			}

			// Check if the pattern matched
			if (pi != pattern.Length)
			{
				return int.MinValue;
			}

			// Score remaining unmatched letters
			score -= (str.Length - si) * 1;

			return Mathf.Max(score, bestScore);
		}
	}
}
