using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[SerializeField]
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

		[SerializeField]
		private List<SearchResult> results = new List<SearchResult>();

		private Texture2D highlight;

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

			bool doSearch = false;
			if (results == null || string.IsNullOrEmpty(newSearchStr))
			{
				results = policy.SearchItems;
				doSearch = true;
			}

			if (doSearch || searchStr != newSearchStr)
			{
				// Restart the search if the new string is shorter
				if (newSearchStr.Length < searchStr.Length)
				{
					results = policy.SearchItems;
				}

				// Restart the search if the new string is a different sequence
				else if (newSearchStr.Substring(0, searchStr.Length) != searchStr)
				{
					results = policy.SearchItems;
				}

				results = (
					from result in results
					select new
					{
						S = FuzzySearch(newSearchStr, result.Label),
						R = result
					})
					.Where(x => x.S != int.MinValue)
					.OrderByDescending(x => x.S)
					.Select(x => x.R)
					.ToList();
			}
			searchStr = newSearchStr;

			selected = Mathf.Clamp(selected, 0, results.Count - 1);
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
			scrollPos = Mathf.Clamp(scrollPos, 0, Mathf.Max(0, results.Count - 12));

			GUILayout.Label("" + results.Count, GUILayout.ExpandWidth(false));
			GUILayout.EndHorizontal();
			
			var oldRichText = GUI.skin.label.richText;
			GUI.skin.label.richText = true;

			GUILayout.BeginHorizontal();
			var scrollSize = GUI.skin.verticalScrollbar.fixedWidth;
			GUILayout.BeginVertical();
			// Show results
			int index = Mathf.Clamp((int)scrollPos, 0, results.Count - 1);
			bool hoveringOnResult = false;
			for (int i = index; i < Mathf.Min(index + 12, results.Count); ++i)
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
			if (results.Count == 0) {
				var oldAlignment = GUI.skin.label.alignment;
				GUI.skin.label.alignment = TextAnchor.LowerCenter;
				GUI.enabled = false;
				GUILayout.Label("<i>No results</i>", GUILayout.MaxHeight(30));
				GUI.enabled = true;
				GUI.skin.label.alignment = oldAlignment;
			}
			GUILayout.EndVertical();
			scrollPos = GUILayout.VerticalScrollbar
				(scrollPos, Mathf.Max(results.Count, 12),
				0, Mathf.Max(results.Count, 12), GUILayout.ExpandHeight(true));
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
