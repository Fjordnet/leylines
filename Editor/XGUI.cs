using UnityEditor;
using UnityEngine;

namespace Exodrifter.NodeGraph
{
	/// <summary>
	/// A helper utility class for sandboxing Unity Editor GUI calls.
	/// </summary>
	public static class XGUI
	{
		#region GUIStyle

		/// <summary>
		/// The style to use when drawing.
		/// </summary>
		private static GUIStyle style = GUIStyle.none;

		/// <summary>
		/// Text alignment.
		/// </summary>
		public static TextAnchor Alignment
		{
			get { return style.alignment; }
			set { style.alignment = value; }
		}

		/// <summary>
		/// The borders of all background images.
		/// </summary>
		public static RectOffset Border
		{
			get { return style.border; }
			set { style.border = value; }
		}

		/// <summary>
		/// What to do when the contents to be rendered is too large to fit
		/// within the area given.
		/// </summary>
		public static TextClipping Clipping
		{
			get { return style.clipping; }
			set { style.clipping = value; }
		}

		/// <summary>
		/// Pixel offset to apply to the content of this GUIstyle.
		/// </summary>
		public static Vector2 ContentOffset
		{
			get { return style.contentOffset; }
			set { style.contentOffset = value; }
		}

		/// <summary>
		/// If non-0, any GUI elements rendered with this style will have the
		/// height specified here.
		/// </summary>
		public static float FixedHeight
		{
			get { return style.fixedHeight; }
			set { style.fixedHeight = value; }
		}

		/// <summary>
		/// If non-0, any GUI elements rendered with this style will have the
		/// width specified here.
		/// </summary>
		public static float FixedWidth
		{
			get { return style.fixedWidth; }
			set { style.fixedWidth = value; }
		}

		/// <summary>
		/// The font to use for rendering. If null, the default font for the
		/// current GUISkin is used instead.
		/// </summary>
		public static Font Font
		{
			get { return style.font; }
			set { style.font = value; }
		}

		/// <summary>
		/// The font size to use (for dynamic fonts).
		/// </summary>
		public static int FontSize
		{
			get { return style.fontSize; }
			set { style.fontSize = value; }
		}

		/// <summary>
		/// The font style to use (for dynamic fonts).
		/// </summary>
		public static FontStyle FontStyle
		{
			get { return style.fontStyle; }
			set { style.fontStyle = value; }
		}

		/// <summary>
		/// How image and text of the GUIContent is combined.
		/// </summary>
		public static ImagePosition ImagePosition
		{
			get { return style.imagePosition; }
			set { style.imagePosition = value; }
		}

		/// <summary>
		/// The height of one line of text with this style, measured in pixels.
		/// (Read Only)
		/// </summary>
		public static float LineHeight
		{
			get { return style.lineHeight; }
		}

		/// <summary>
		/// The margins between elements rendered in this style and any other GUI elements.
		/// </summary>
		public static RectOffset Margin
		{
			get { return style.margin; }
			set { style.margin = value; }
		}

		/// <summary>
		/// The name of this GUIStyle. Used for getting them based on name.
		/// </summary>
		public static string Name
		{
			get { return style.name; }
			set { style.name = value; }
		}

		/// <summary>
		/// Extra space to be added to the background image.
		/// </summary>
		public static RectOffset Overflow
		{
			get { return style.overflow; }
			set { style.overflow = value; }
		}

		/// <summary>
		/// Space from the edge of GUIStyle to the start of the contents.
		/// </summary>
		public static RectOffset Padding
		{
			get { return style.padding; }
			set { style.padding = value; }
		}

		/// <summary>
		/// Enable HTML-style tags for Text Formatting Markup.
		/// </summary>
		public static bool RichText
		{
			get { return style.richText; }
			set { style.richText = value; }
		}

		/// <summary>
		/// Can GUI elements of this style be stretched vertically for better
		/// layout?
		/// </summary>
		public static bool StretchHeight
		{
			get { return style.stretchHeight; }
			set { style.stretchHeight = value; }
		}

		/// <summary>
		/// Can GUI elements of this style be stretched horizontally for better
		/// layouting?
		/// </summary>
		public static bool StretchWidth
		{
			get { return style.stretchWidth; }
			set { style.stretchWidth = value; }
		}

		/// <summary>
		/// Should the text be wordwrapped?
		/// </summary>
		public static bool WordWrap
		{
			get { return style.wordWrap; }
			set { style.wordWrap = value; }
		}

		/// <summary>
		/// Rendering settings for when the control is pressed down.
		/// </summary>
		public static GUIStyleState Active
		{
			get { return style.active; }
			set { style.active = value; }
		}

		/// <summary>
		/// Rendering settings for when the element has keyboard focus.
		/// </summary>
		public static GUIStyleState Focused
		{
			get { return style.focused; }
			set { style.focused = value; }
		}

		/// <summary>
		/// Rendering settings for when the mouse is hovering over the control.
		/// </summary>
		public static GUIStyleState Hover
		{
			get { return style.hover; }
			set { style.hover = value; }
		}

		/// <summary>
		/// Rendering settings for when the component is displayed normally.
		/// </summary>
		public static GUIStyleState Normal
		{
			get { return style.normal; }
			set { style.normal = value; }
		}

		/// <summary>
		/// Rendering settings for when the element is turned on and pressed
		/// down.
		/// </summary>
		public static GUIStyleState OnActive
		{
			get { return style.onActive; }
			set { style.onActive = value; }
		}

		/// <summary>
		/// Rendering settings for when the element has keyboard and is turned
		/// on.
		/// </summary>
		public static GUIStyleState OnFocused
		{
			get { return style.onFocused; }
			set { style.onFocused = value; }
		}

		/// <summary>
		/// Rendering settings for when the control is turned on and the mouse
		/// is hovering it.
		/// </summary>
		public static GUIStyleState OnHover
		{
			get { return style.onHover; }
			set { style.onHover = value; }
		}

		/// <summary>
		/// Rendering settings for when the control is turned on.
		/// </summary>
		public static GUIStyleState OnNormal
		{
			get { return style.onNormal; }
			set { style.onNormal = value; }
		}

		#endregion

		#region Static State

		/// <summary>
		/// Global tinting color for all background elements rendered by the
		/// GUI.
		/// </summary>
		public static Color BackgroundColor
		{
			get { return backgroundColor; }
			set { backgroundColor = value; }
		}
		private static Color backgroundColor = Color.white;

		/// <summary>
		/// Returns true if any controls changed the value of the input data.
		/// Not reset when ResetToStyle is called.
		/// </summary>
		public static bool Changed
		{
			get { return GUI.changed; }
			set { GUI.changed = value; }
		}

		/// <summary>
		/// Global tinting color for the GUI.
		/// </summary>
		public static Color Color
		{
			get { return color; }
			set { color = value; }
		}
		private static Color color = Color.white;

		/// <summary>
		/// Tinting color for all text rendered by the GUI.
		/// </summary>
		public static Color ContentColor
		{
			get { return contentColor; }
			set { contentColor = value; }
		}
		private static Color contentColor = Color.white;

		/// <summary>
		/// Is the GUI enabled?
		/// </summary>
		public static bool Enabled
		{
			get { return enabled; }
			set { enabled = value; }
		}
		private static bool enabled = true;

		/// <summary>
		/// The GUI transform matrix.
		/// </summary>
		public static Matrix4x4 Matrix
		{
			get { return matrix; }
			set { matrix = value; }
		}
		private static Matrix4x4 matrix = Matrix4x4.identity;

		/// <summary>
		/// The global skin to use. Not reset when ResetToStyle is called.
		/// </summary>
		public static GUISkin Skin
		{
			get { return GUI.skin; }
			set { GUI.skin = value; }
		}

		/// <summary>
		/// The tooltip of the control the mouse is currently over, or which
		/// has keyboard focus. (Read Only).
		/// </summary>
		public static string ToolTip
		{
			get { return GUI.tooltip; }
		}

		/// <summary>
		/// Resets the static state.
		/// </summary>
		private static void ResetStatic()
		{
			enabled = true;
			color = Color.white;
			backgroundColor = Color.white;
			contentColor = Color.white;
			matrix = Matrix4x4.identity;
		}

		#endregion

		#region GUI & GUILayout

		public static void BeginArea(Rect screenRect)
		{
			using (new XGUIStatic())
				GUILayout.BeginArea(screenRect, style);
		}

		public static void BeginArea(Rect screenRect, string text)
		{
			using (new XGUIStatic())
				GUILayout.BeginArea(screenRect, text, style);
		}

		public static void BeginArea(Rect screenRect, Texture image)
		{
			using (new XGUIStatic())
				GUILayout.BeginArea(screenRect, image, style);
		}

		public static void BeginArea(Rect screenRect, GUIContent content)
		{
			using (new XGUIStatic())
				GUILayout.BeginArea(screenRect, content, style);
		}

		public static void BeginClip(Rect rect)
		{
			using (new XGUIStatic())
				GUI.BeginClip(rect);
		}

		public static void BeginClip(Rect rect, Vector2 scrollOffset, Vector2 renderOffset, bool resetOffset)
		{
			using (new XGUIStatic())
				GUI.BeginClip(rect, scrollOffset, renderOffset, resetOffset);
		}

		public static void BeginGroup(Rect rect)
		{
			using (new XGUIStatic())
				GUI.BeginGroup(rect, style);
		}

		public static void BeginGroup(Rect rect, Texture image)
		{
			using (new XGUIStatic())
				GUI.BeginGroup(rect, image, style);
		}

		public static void BeginGroup(Rect rect, GUIContent content)
		{
			using (new XGUIStatic())
				GUI.BeginGroup(rect, content, style);
		}

		public static void BeginGroup(Rect rect, string text)
		{
			using (new XGUIStatic())
				GUI.BeginGroup(rect, text, style);
		}

		public static void BeginHorizontal(params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginHorizontal(style, options);
		}

		public static void BeginHorizontal
			(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginHorizontal(text, style, options);
		}

		public static void BeginHorizontal
			(Texture image, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginHorizontal(image, style, options);
		}

		public static void BeginHorizontal
			(GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginHorizontal(content, style, options);
		}

		public static void BeginVertical(params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginVertical(style, options);
		}

		public static void BeginVertical(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginVertical(text, style, options);
		}

		public static void BeginVertical(Texture image, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginVertical(image, style, options);
		}

		public static void BeginVertical(GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.BeginVertical(content, style, options);
		}

		public static void Box(Rect rect)
		{
			using (new XGUIStatic())
				GUI.Box(rect, GUIContent.none, style);
		}

		public static void Box(Rect rect, string text)
		{
			using (new XGUIStatic())
				GUI.Box(rect, text, style);
		}

		public static void Box(Rect rect, Texture image)
		{
			using (new XGUIStatic())
				GUI.Box(rect, image, style);
		}

		public static void Box(Rect rect, GUIContent content)
		{
			using (new XGUIStatic())
				GUI.Box(rect, content, style);
		}

		public static void Box(params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Box(GUIContent.none, style, options);
		}

		public static void Box(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Box(text, style, options);
		}

		public static void Box(Texture image, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Box(image, style, options);
		}

		public static void Box(GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Box(content, style, options);
		}

		public static bool Button(Rect rect, string text)
		{
			using (new XGUIStatic())
				return GUI.Button(rect, text, style);
		}

		public static bool Button(Rect rect, Texture image)
		{
			using (new XGUIStatic())
				return GUI.Button(rect, image, style);
		}

		public static bool Button(Rect rect, GUIContent content)
		{
			using (new XGUIStatic())
				return GUI.Button(rect, content, style);
		}

		public static bool Button(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Button(text, style, options);
		}

		public static bool Button(Texture image, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Button(image, style, options);
		}

		public static bool Button(GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Button(content, style, options);
		}

		public static void EndArea()
		{
			GUILayout.EndArea();
		}

		public static void EndClip()
		{
			GUI.EndClip();
		}

		public static void EndGroup()
		{
			GUI.EndGroup();
		}

		public static void EndHorizontal()
		{
			GUILayout.EndHorizontal();
		}

		public static void EndVertical()
		{
			GUILayout.EndVertical();
		}

		public static void FlexibleSpace()
		{
			using (new XGUIStatic())
				GUILayout.FlexibleSpace();
		}

		public static void Label(Rect rect, string text)
		{
			using (new XGUIStatic())
				GUI.Label(rect, text, style);
		}

		public static void Label(Rect rect, Texture image)
		{
			using (new XGUIStatic())
				GUI.Label(rect, image, style);
		}

		public static void Label(Rect rect, GUIContent content)
		{
			using (new XGUIStatic())
				GUI.Label(rect, content, style);
		}

		public static void Label(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Label(text, style, options);
		}

		public static void Label(Texture texture, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Label(texture, style, options);
		}

		public static void Label(GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				GUILayout.Label(content, style, options);
		}

		public static string PasswordField(Rect rect, string password, char maskChar)
		{
			using (new XGUIStatic())
				return GUI.PasswordField(rect, password, maskChar, style);
		}

		public static string PasswordField(Rect rect, string password, char maskChar, int maxLength)
		{
			using (new XGUIStatic())
				return GUI.PasswordField(rect, password, maskChar, maxLength, style);
		}

		public static string PasswordField(string password, char maskChar)
		{
			using (new XGUIStatic())
				return GUILayout.PasswordField(password, maskChar, style);
		}

		public static string PasswordField(string password, char maskChar, int maxLength)
		{
			using (new XGUIStatic())
				return GUILayout.PasswordField(password, maskChar, maxLength, style);
		}

		public static bool RepeatButton(Rect rect, string text)
		{
			using (new XGUIStatic())
				return GUI.RepeatButton(rect, text, style);
		}

		public static bool RepeatButton(Rect rect, Texture image)
		{
			using (new XGUIStatic())
				return GUI.RepeatButton(rect, image, style);
		}

		public static bool RepeatButton(Rect rect, GUIContent content)
		{
			using (new XGUIStatic())
				return GUI.RepeatButton(rect, content, style);
		}

		public static bool RepeatButton(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.RepeatButton(text, style, options);
		}

		public static bool RepeatButton(Texture image, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.RepeatButton(image, style, options);
		}

		public static bool RepeatButton(GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.RepeatButton(content, style, options);
		}

		public static int SelectionGrid(Rect rect, int selected, string[] texts, int xCount)
		{
			using (new XGUIStatic())
				return GUI.SelectionGrid(rect, selected, texts, xCount, style);
		}

		public static int SelectionGrid(Rect rect, int selected, Texture[] images, int xCount)
		{
			using (new XGUIStatic())
				return GUI.SelectionGrid(rect, selected, images, xCount, style);
		}

		public static int SelectionGrid(Rect rect, int selected, GUIContent[] contents, int xCount)
		{
			using (new XGUIStatic())
				return GUI.SelectionGrid(rect, selected, contents, xCount, style);
		}

		public static int SelectionGrid(int selected, string[] texts, int xCount, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.SelectionGrid(selected, texts, xCount, style, options);
		}

		public static int SelectionGrid(int selected, Texture[] images, int xCount, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.SelectionGrid(selected, images, xCount, style, options);
		}

		public static int SelectionGrid(int selected, GUIContent[] contents, int xCount, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.SelectionGrid(selected, contents, xCount, style, options);
		}

		public static string TextArea(Rect rect, string text)
		{
			using (new XGUIStatic())
				return GUI.TextArea(rect, text, style);
		}

		public static string TextArea(Rect rect, string text, int maxLength)
		{
			using (new XGUIStatic())
				return GUI.TextArea(rect, text, maxLength, style);
		}

		public static string TextArea(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.TextArea(text, style, options);
		}

		public static string TextArea(string text, int maxLength, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.TextArea(text, maxLength, style, options);
		}

		public static string TextField(Rect rect, string text)
		{
			using (new XGUIStatic())
				return GUI.TextField(rect, text, style);
		}

		public static string TextField(Rect rect, string text, int maxLength)
		{
			using (new XGUIStatic())
				return GUI.TextField(rect, text, maxLength, style);
		}

		public static string TextField(string text, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.TextField(text, style, options);
		}

		public static string TextField(string text, int maxLength, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.TextField(text, maxLength, style, options);
		}

		public static bool ToggleButton(Rect rect, bool value, string label)
		{
			using (new XGUIStatic())
				return GUI.Toggle(rect, value, label, style);
		}

		public static bool ToggleButton(Rect rect, bool value, GUIContent content)
		{
			using (new XGUIStatic())
				return GUI.Toggle(rect, value, content, style);
		}

		public static bool ToggleButton(bool value, string label, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Toggle(value, label, style, options);
		}

		public static bool ToggleButton(bool value, Texture image, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Toggle(value, image, style, options);
		}

		public static bool ToggleButton(bool value, GUIContent content, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Toggle(value, content, style, options);
		}

		public static int Toolbar(Rect rect, int selected, string[] texts)
		{
			using (new XGUIStatic())
				return GUI.Toolbar(rect, selected, texts, style);
		}

		public static int Toolbar(Rect rect, int selected, Texture[] images)
		{
			using (new XGUIStatic())
				return GUI.Toolbar(rect, selected, images, style);
		}

		public static int Toolbar(Rect rect, int selected, GUIContent[] contents)
		{
			using (new XGUIStatic())
				return GUI.Toolbar(rect, selected, contents, style);
		}

		public static int Toolbar(int selected, string[] texts, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Toolbar(selected, texts, style, options);
		}

		public static int Toolbar(int selected, Texture[] images, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Toolbar(selected, images, style, options);
		}

		public static int Toolbar(int selected, GUIContent[] contents, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return GUILayout.Toolbar(selected, contents, style, options);
		}

		#endregion

		#region EditorGUI & EditorGUILayout

		#region BoundsField

		public static Bounds BoundsField(Rect rect, Bounds value)
		{
			using (new XGUIStatic())
				return EditorGUI.BoundsField(rect, value);
		}

		public static Bounds BoundsField(Rect rect, string label, Bounds value)
		{
			using (new XGUIStatic())
				return EditorGUI.BoundsField(rect, label, value);
		}

		public static Bounds BoundsField(Rect rect, GUIContent label, Bounds value)
		{
			using (new XGUIStatic())
				return EditorGUI.BoundsField(rect, label, value);
		}

		public static Bounds BoundsField(Bounds value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.BoundsField(value, options);
		}

		public static Bounds BoundsField(string label, Bounds value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.BoundsField(label, value, options);
		}

		public static Bounds BoundsField(GUIContent label, Bounds value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.BoundsField(label, value, options);
		}

		#endregion

		#region ColorField

		public static Color ColorField(Rect rect, Color value)
		{
			using (new XGUIStatic())
				return EditorGUI.ColorField(rect, value);
		}

		public static Color ColorField(Rect rect, string label, Color value)
		{
			using (new XGUIStatic())
				return EditorGUI.ColorField(rect, label, value);
		}

		public static Color ColorField(Rect rect, GUIContent label, Color value)
		{
			using (new XGUIStatic())
				return EditorGUI.ColorField(rect, label, value);
		}

		public static Color ColorField(Rect rect, string label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
		{
			using (new XGUIStatic())
				return EditorGUI.ColorField(rect, new GUIContent(label), value, showEyedropper, showAlpha, hdr, hdrConfig);
		}

		public static Color ColorField(Rect rect, GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig)
		{
			using (new XGUIStatic())
				return EditorGUI.ColorField(rect, label, value, showEyedropper, showAlpha, hdr, hdrConfig);
		}

		public static Color ColorField(Color value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ColorField(value, options);
		}

		public static Color ColorField(string label, Color value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ColorField(label, value, options);
		}

		public static Color ColorField(GUIContent label, Color value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ColorField(label, value, options);
		}

		public static Color ColorField(string label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ColorField(new GUIContent(label), value, showEyedropper, showAlpha, hdr, hdrConfig, options);
		}

		public static Color ColorField(GUIContent label, Color value, bool showEyedropper, bool showAlpha, bool hdr, ColorPickerHDRConfig hdrConfig, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ColorField(label, value, showEyedropper, showAlpha, hdr, hdrConfig, options);
		}

		#endregion

		#region CurveField

		public static AnimationCurve CurveField(Rect rect, AnimationCurve value)
		{
			using (new XGUIStatic())
				return EditorGUI.CurveField(rect, value);
		}

		public static AnimationCurve CurveField(Rect rect, string label, AnimationCurve value)
		{
			using (new XGUIStatic())
				return EditorGUI.CurveField(rect, label, value);
		}

		public static AnimationCurve CurveField(Rect rect, GUIContent label, AnimationCurve value)
		{
			using (new XGUIStatic())
				return EditorGUI.CurveField(rect, label, value);
		}

		public static AnimationCurve CurveField(Rect rect, AnimationCurve value, Color color, Rect ranges)
		{
			using (new XGUIStatic())
				return EditorGUI.CurveField(rect, value, color, ranges);
		}

		public static AnimationCurve CurveField(Rect rect, string label, AnimationCurve value, Color color, Rect ranges)
		{
			using (new XGUIStatic())
				return EditorGUI.CurveField(rect, label, value, color, ranges);
		}

		public static AnimationCurve CurveField(Rect rect, GUIContent label, AnimationCurve value, Color color, Rect ranges)
		{
			using (new XGUIStatic())
				return EditorGUI.CurveField(rect, label, value, color, ranges);
		}

		public static void CurveField(Rect rect, SerializedProperty prop, Color color, Rect ranges)
		{
			using (new XGUIStatic())
				EditorGUI.CurveField(rect, prop, color, ranges);
		}

		public static void CurveField(Rect rect, string label, SerializedProperty prop, Color color, Rect ranges)
		{
			using (new XGUIStatic())
				EditorGUI.CurveField(rect, prop, color, ranges, new GUIContent(label));
		}

		public static void CurveField(Rect rect, GUIContent label, SerializedProperty prop, Color color, Rect ranges)
		{
			using (new XGUIStatic())
				EditorGUI.CurveField(rect, prop, color, ranges, label);
		}

		public static AnimationCurve CurveField(AnimationCurve value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.CurveField(value, options);
		}

		public static AnimationCurve CurveField(string label, AnimationCurve value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.CurveField(label, value, options);
		}

		public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.CurveField(label, value, options);
		}

		public static AnimationCurve CurveField(AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.CurveField(value, color, ranges, options);
		}

		public static AnimationCurve CurveField(string label, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.CurveField(label, value, color, ranges, options);
		}

		public static AnimationCurve CurveField(GUIContent label, AnimationCurve value, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.CurveField(label, value, color, ranges, options);
		}

		public static void CurveField(SerializedProperty prop, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.CurveField(prop, color, ranges, options);
		}

		public static void CurveField(string label, SerializedProperty prop, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.CurveField(prop, color, ranges, new GUIContent(label), options);
		}

		public static void CurveField(GUIContent label, SerializedProperty prop, Color color, Rect ranges, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.CurveField(prop, color, ranges, label, options);
		}

		#endregion

		#region DelayedDoubleField

		public static double DelayedDoubleField(Rect rect, double value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedDoubleField(rect, value, style);
		}

		public static double DelayedDoubleField(Rect rect, string label, double value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedDoubleField(rect, label, value, style);
		}

		public static double DelayedDoubleField(Rect rect, GUIContent label, double value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedDoubleField(rect, label, value, style);
		}

		public static double DelayedDoubleField(double value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedDoubleField(value, style, options);
		}

		public static double DelayedDoubleField(string label, double value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedDoubleField(label, value, style, options);
		}

		public static double DelayedDoubleField(GUIContent label, double value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedDoubleField(label, value, style, options);
		}

		#endregion

		#region DelayedFloatField

		public static float DelayedFloatField(Rect rect, float value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedFloatField(rect, value, style);
		}

		public static float DelayedFloatField(Rect rect, string label, float value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedFloatField(rect, label, value, style);
		}

		public static float DelayedFloatField(Rect rect, GUIContent label, float value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedFloatField(rect, label, value, style);
		}

		public static void DelayedFloatField(Rect rect, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedFloatField(rect, prop);
		}

		public static void DelayedFloatField(Rect rect, string label, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedFloatField(rect, prop, new GUIContent(label));
		}

		public static void DelayedFloatField(Rect rect, GUIContent label, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedFloatField(rect, prop, label);
		}

		public static float DelayedFloatField(float value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedFloatField(value, style, options);
		}

		public static float DelayedFloatField(string label, float value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedFloatField(label, value, style, options);
		}

		public static float DelayedFloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedFloatField(label, value, style, options);
		}

		public static void DelayedFloatField(SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedFloatField(prop, options);
		}

		public static void DelayedFloatField(string label, SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedFloatField(prop, new GUIContent(label), options);
		}

		public static void DelayedFloatField(GUIContent label, SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedFloatField(prop, label, options);
		}

		#endregion

		#region DelayedIntField

		public static int DelayedIntField(Rect rect, int value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedIntField(rect, value, style);
		}

		public static int DelayedIntField(Rect rect, string label, int value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedIntField(rect, label, value, style);
		}

		public static int DelayedIntField(Rect rect, GUIContent label, int value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedIntField(rect, label, value, style);
		}

		public static void DelayedIntField(Rect rect, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedIntField(rect, prop);
		}

		public static void DelayedIntField(Rect rect, string label, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedIntField(rect, prop, new GUIContent(label));
		}

		public static void DelayedIntField(Rect rect, GUIContent label, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedIntField(rect, prop, label);
		}

		public static int DelayedIntField(int value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedIntField(value, style, options);
		}

		public static int DelayedIntField(string label, int value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedIntField(label, value, style, options);
		}

		public static int DelayedIntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedIntField(label, value, style, options);
		}

		public static void DelayedIntField(SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedIntField(prop, options);
		}

		public static void DelayedIntField(string label, SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedIntField(prop, new GUIContent(label), options);
		}

		public static void DelayedIntField(GUIContent label, SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedIntField(prop, label, options);
		}

		#endregion

		#region DelayedTextField

		public static string DelayedTextField(Rect rect, string value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedTextField(rect, value, style);
		}

		public static string DelayedTextField(Rect rect, string label, string value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedTextField(rect, label, value, style);
		}

		public static string DelayedTextField(Rect rect, GUIContent label, string value)
		{
			using (new XGUIStatic())
				return EditorGUI.DelayedTextField(rect, label, value, style);
		}

		public static void DelayedTextField(Rect rect, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedTextField(rect, prop);
		}

		public static void DelayedTextField(Rect rect, string label, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedTextField(rect, prop, new GUIContent(label));
		}

		public static void DelayedTextField(Rect rect, GUIContent label, SerializedProperty prop)
		{
			using (new XGUIStatic())
				EditorGUI.DelayedTextField(rect, prop, label);
		}

		public static string DelayedTextField(string value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedTextField(value, style, options);
		}

		public static string DelayedTextField(string label, string value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedTextField(label, value, style, options);
		}

		public static string DelayedTextField(GUIContent label, string value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DelayedTextField(label, value, style, options);
		}

		public static void DelayedTextField(SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedTextField(prop, options);
		}

		public static void DelayedTextField(string label, SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedTextField(prop, new GUIContent(label), options);
		}

		public static void DelayedTextField(GUIContent label, SerializedProperty prop, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.DelayedTextField(prop, label, options);
		}

		#endregion

		#region DoubleField

		public static double DoubleField(Rect rect, double value)
		{
			using (new XGUIStatic())
				return EditorGUI.DoubleField(rect, value, style);
		}

		public static double DoubleField(Rect rect, string label, double value)
		{
			using (new XGUIStatic())
				return EditorGUI.DoubleField(rect, label, value, style);
		}

		public static double DoubleField(Rect rect, GUIContent label, double value)
		{
			using (new XGUIStatic())
				return EditorGUI.DoubleField(rect, label, value, style);
		}

		public static double DoubleField(double value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DoubleField(value, style, options);
		}

		public static double DoubleField(string label, double value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DoubleField(label, value, style, options);
		}

		public static double DoubleField(GUIContent label, double value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.DoubleField(label, value, style, options);
		}

		#endregion

		#region ObjectField

		public static Object ObjectField(Rect rect, Object obj, System.Type objType, bool allowSceneObjects)
		{
			using (new XGUIStatic())
				return EditorGUI.ObjectField(rect, obj, objType, allowSceneObjects);
		}

		public static Object ObjectField(Rect rect, string label, Object obj, System.Type objType, bool allowSceneObjects)
		{
			using (new XGUIStatic())
				return EditorGUI.ObjectField(rect, label, obj, objType, allowSceneObjects);
		}

		public static Object ObjectField(Rect rect, GUIContent label, Object obj, System.Type objType, bool allowSceneObjects)
		{
			using (new XGUIStatic())
				return EditorGUI.ObjectField(rect, label, obj, objType, allowSceneObjects);
		}

		public static T ObjectField<T>(Rect rect, T obj, bool allowSceneObjects) where T : Object
		{
			using (new XGUIStatic())
				return (T)EditorGUI.ObjectField(rect, obj, typeof(T), allowSceneObjects);
		}

		public static T ObjectField<T>(Rect rect, string label, T obj, bool allowSceneObjects) where T : Object
		{
			using (new XGUIStatic())
				return (T)EditorGUI.ObjectField(rect, label, obj, typeof(T), allowSceneObjects);
		}

		public static T ObjectField<T>(Rect rect, GUIContent label, T obj, bool allowSceneObjects) where T : Object
		{
			using (new XGUIStatic())
				return (T)EditorGUI.ObjectField(rect, label, obj, typeof(T), allowSceneObjects);
		}

		public static void ObjectField(Rect rect, SerializedProperty prop, System.Type objType, bool allowSceneObjects)
		{
			using (new XGUIStatic())
				EditorGUI.ObjectField(rect, prop, objType);
		}

		public static void ObjectField(Rect rect, string label, SerializedProperty prop, System.Type objType, bool allowSceneObjects)
		{
			using (new XGUIStatic())
				EditorGUI.ObjectField(rect, prop, objType, new GUIContent(label));
		}

		public static void ObjectField(Rect rect, GUIContent label, SerializedProperty prop, System.Type objType, bool allowSceneObjects)
		{
			using (new XGUIStatic())
				EditorGUI.ObjectField(rect, prop, objType, label);
		}

		public static void ObjectField<T>(Rect rect, SerializedProperty prop, bool allowSceneObjects) where T : Object
		{
			using (new XGUIStatic())
				EditorGUI.ObjectField(rect, prop, typeof(T));
		}

		public static void ObjectField<T>(Rect rect, string label, SerializedProperty prop, bool allowSceneObjects) where T : Object
		{
			using (new XGUIStatic())
				EditorGUI.ObjectField(rect, prop, typeof(T), new GUIContent(label));
		}

		public static void ObjectField<T>(Rect rect, GUIContent label, SerializedProperty prop, bool allowSceneObjects) where T : Object
		{
			using (new XGUIStatic())
				EditorGUI.ObjectField(rect, prop, typeof(T), label);
		}

		public static Object ObjectField(Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ObjectField(obj, objType, allowSceneObjects, options);
		}

		public static Object ObjectField(string label, Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjects, options);
		}

		public static Object ObjectField(GUIContent label, Object obj, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ObjectField(label, obj, objType, allowSceneObjects, options);
		}

		public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			using (new XGUIStatic())
				return (T)EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options);
		}

		public static T ObjectField<T>(string label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			using (new XGUIStatic())
				return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
		}

		public static T ObjectField<T>(GUIContent label, T obj, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			using (new XGUIStatic())
				return (T)EditorGUILayout.ObjectField(label, obj, typeof(T), allowSceneObjects, options);
		}

		public static void ObjectField(SerializedProperty prop, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.ObjectField(prop, objType, options);
		}

		public static void ObjectField(string label, SerializedProperty prop, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.ObjectField(prop, objType, new GUIContent(label), options);
		}

		public static void ObjectField(GUIContent label, SerializedProperty prop, System.Type objType, bool allowSceneObjects, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				EditorGUILayout.ObjectField(prop, objType, label, options);
		}

		public static void ObjectField<T>(SerializedProperty prop, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			using (new XGUIStatic())
				EditorGUILayout.ObjectField(prop, typeof(T), options);
		}

		public static void ObjectField<T>(string label, SerializedProperty prop, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			using (new XGUIStatic())
				EditorGUILayout.ObjectField(prop, typeof(T), new GUIContent(label), options);
		}

		public static void ObjectField<T>(GUIContent label, SerializedProperty prop, bool allowSceneObjects, params GUILayoutOption[] options) where T : Object
		{
			using (new XGUIStatic())
				EditorGUILayout.ObjectField(prop, typeof(T), label, options);
		}

		#endregion

		#region ToggleLeft

		public static bool ToggleLeft(Rect rect, string label, bool value)
		{
			using (new XGUIStatic())
				return EditorGUI.ToggleLeft(rect, label, value, style);
		}

		public static bool ToggleLeft(Rect rect, GUIContent label, bool value)
		{
			using (new XGUIStatic())
				return EditorGUI.ToggleLeft(rect, label, value, style);
		}

		public static bool ToggleLeft(string label, bool value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ToggleLeft(label, value, style, options);
		}

		public static bool ToggleLeft(GUIContent label, bool value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.ToggleLeft(label, value, style, options);
		}

		#endregion

		#region ToggleRight

		public static bool ToggleRight(Rect rect, bool value)
		{
			using (new XGUIStatic())
				return EditorGUI.Toggle(rect, value, style);
		}

		public static bool ToggleRight(Rect rect, string label, bool value)
		{
			using (new XGUIStatic())
				return EditorGUI.Toggle(rect, label, value, style);
		}

		public static bool ToggleRight(Rect rect, GUIContent label, bool value)
		{
			using (new XGUIStatic())
				return EditorGUI.Toggle(rect, label, value, style);
		}

		public static bool ToggleRight(bool value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.Toggle(value, style, options);
		}

		public static bool ToggleRight(string label, bool value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.Toggle(label, value, style, options);
		}

		public static bool ToggleRight(GUIContent label, bool value, params GUILayoutOption[] options)
		{
			using (new XGUIStatic())
				return EditorGUILayout.Toggle(label, value, style, options);
		}

		#endregion

		#endregion

		#region GUI Helpers

		/// <summary>
		/// Shorthand for empty content.
		/// </summary>
		public static GUIContent None { get { return GUIContent.none; } }

		/// <summary>
		/// Option passed to a control to allow or disallow vertical expansion.
		/// </summary>
		public static GUILayoutOption ExpandHeight(bool expand)
		{
			return GUILayout.ExpandHeight(expand);
		}

		/// <summary>
		/// Option passed to a control to allow or disallow horizontal
		/// expansion.
		/// </summary>
		public static GUILayoutOption ExpandWidth(bool expand)
		{
			return GUILayout.ExpandWidth(expand);
		}

		/// <summary>
		/// Option passed to a control to give it an absolute height.
		/// </summary>
		public static GUILayoutOption Height(float height)
		{
			return GUILayout.Height(height);
		}

		/// <summary>
		/// Option passed to a control to specify a maximum height.
		/// </summary>
		public static GUILayoutOption MaxHeight(float maxHeight)
		{
			return GUILayout.MaxHeight(maxHeight);
		}

		/// <summary>
		/// Option passed to a control to specify a maximum width.
		/// </summary>
		public static GUILayoutOption MaxWidth(float maxWidth)
		{
			return GUILayout.MaxWidth(maxWidth);
		}

		/// <summary>
		/// Option passed to a control to specify a minimum height.
		/// </summary>
		public static GUILayoutOption MinHeight(float minHeight)
		{
			return GUILayout.MinHeight(minHeight);
		}

		/// <summary>
		/// Option passed to a control to specify a minimum width.
		/// </summary>
		public static GUILayoutOption MinWidth(float minWidth)
		{
			return GUILayout.MinWidth(minWidth);
		}

		/// <summary>
		/// Option passed to a control to give it an absolute width.
		/// </summary>
		public static GUILayoutOption Width(float width)
		{
			return GUILayout.Width(width);
		}

		#endregion

		#region Methods

		/// <summary>
		/// Copies the passed style for use in future calls and resets all of
		/// the static variables to their default values.
		/// </summary>
		/// <param name="style">The style to use in future calls.</param>
		public static void ResetToStyle(GUIStyle style)
		{
			XGUI.style = new GUIStyle(style ?? GUIStyle.none);
			ResetStatic();
		}

		#endregion
	}
}
