using System;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	[CreateAssetMenu(fileName = "Node Graph Skin", menuName = "Node Graph Skin")]
	public class NodeGraphSkin : ScriptableObject
	{
		[Header("Canvas")]
		public Color canvasColor;
		public Color canvasAxisLineColor;
		public Color canvasCellLineColor;
		public Color canvasChunkLineColor;

		[Header("Node")]
		[Range(0f, 1f)]
		public float offNodeTint = 0.5f;
		public Color nodeColor;
		public Texture2D nodeTexture;
		public RectOffset nodeTextureOffset;

		[Header("Sockets")]
		[Range(0f, 1f)]
		public float offSocketTint = 0.5f;
		public Color execSocketColor;
		public Color primitiveSocketColor;
		public Color objectSocketColor;

		public SocketStyle paramSocketStyle;
		public SocketStyle execSocketStyle;

		[Header("Links")]
		public Color tempLinkColor;

		public Color TintColor(Color color, float tint)
		{
			return new Color(
				color.r * tint,
				color.g * tint,
				color.b * tint,
				color.a
			);
		}
	}

	[Serializable]
	public class SocketStyle
	{
		public Texture2D offSocketTexture;
		public Texture2D onSocketTexture;
	}
}
