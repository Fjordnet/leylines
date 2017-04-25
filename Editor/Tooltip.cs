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
	/// Stores and renders the text displayed in a tooltip
	/// </summary>
	public class Tooltip
	{
		public string Text { get; set; }

		public Tooltip(string text)
		{
			Text = text;
		}

		internal void OnGUI()
		{
			var content = new GUIContent(Text);

			var richText = GUI.skin.box.richText;
			var color = GUI.skin.box.normal.textColor;
			var alignment = GUI.skin.box.alignment;
			GUI.skin.box.richText = true;
			GUI.skin.box.normal.textColor = Color.white;
			GUI.skin.box.alignment = TextAnchor.UpperLeft;

			var popupRect = new Rect();
			popupRect.xMin = Event.current.mousePosition.x + 10;
			popupRect.yMin = Event.current.mousePosition.y + 10;
			popupRect.size = GUI.skin.box.CalcSize(content);
			GUI.Box(popupRect, content);

			GUI.skin.box.richText = richText;
			GUI.skin.box.normal.textColor = color;
			GUI.skin.box.alignment = alignment;
		}
	}
}
