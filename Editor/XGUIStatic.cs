using System;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	public class XGUIStatic : IDisposable
	{
		private bool oldEnabled;
		private Color oldColor;
		private Color oldBackgroundColor;
		private Color oldContentColor;
		private Matrix4x4 oldMatrix;

		public XGUIStatic()
		{
			oldBackgroundColor = GUI.backgroundColor;
			oldColor = GUI.color;
			oldContentColor = GUI.contentColor;
			oldEnabled = GUI.enabled;
			oldMatrix = GUI.matrix;

			GUI.backgroundColor = XGUI.BackgroundColor;
			GUI.color = XGUI.Color;
			GUI.contentColor = XGUI.ContentColor;
			GUI.enabled = XGUI.Enabled;
			GUI.matrix = XGUI.Matrix;
		}

		public void Dispose()
		{
			GUI.backgroundColor = oldBackgroundColor;
			GUI.color = oldColor;
			GUI.contentColor = oldContentColor;
			GUI.enabled = oldEnabled;
			GUI.matrix = oldMatrix;
		}
	}
}
