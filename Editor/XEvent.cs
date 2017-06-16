using UnityEngine;

namespace Exodrifter.NodeGraph
{
	public static class XEvent
	{
		public static Vector2 MousePos
		{
			get { return Event.current.mousePosition; }
		}

		public static bool AreModifiersPressed(EventModifiers modifiers)
		{
			return (Event.current.modifiers & modifiers) == modifiers;
		}

		public static bool AreModifiersPressed(params EventModifiers[] modifiers)
		{
			var modifier = EventModifiers.None;
			foreach (var mod in modifiers)
			{
				modifier |= mod;
			}
			return (Event.current.modifiers & modifier) == modifier;
		}

		public static float GetScrollWheel()
		{
			return Event.current.delta.y;
		}

		public static bool IsKeyDown(KeyCode? key = null)
		{
			return Event.current.type == EventType.KeyDown
				&& (key == null || Event.current.keyCode == key);
		}

		public static bool IsKeyUp(KeyCode? key = null)
		{
			return Event.current.type == EventType.KeyUp
				&& (key == null || Event.current.keyCode == key);
		}

		public static bool IsMouseDown(int? button = null)
		{
			return Event.current.type == EventType.MouseDown
				&& (button == null || Event.current.button == button);
		}

		public static bool IsMouseDrag(int? button = null)
		{
			return Event.current.type == EventType.MouseDrag
				&& (button == null || Event.current.button == button);
		}

		public static bool IsMouseMove()
		{
			return Event.current.type == EventType.MouseMove;
		}

		public static bool IsMouseUp(int? button = null)
		{
			return Event.current.type == EventType.MouseUp
				&& (button == null || Event.current.button == button);
		}

		public static bool IsRepaint()
		{
			return Event.current.type == EventType.Repaint;
		}

		public static bool IsScrollWheel()
		{
			return Event.current.type == EventType.ScrollWheel;
		}

		public static void Use()
		{
			Event.current.Use();
		}
	}
}