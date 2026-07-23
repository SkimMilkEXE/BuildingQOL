using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace BuildingQOL.Content.Selection
{
	public class UndoSystem : ModSystem
	{
		private class Change
		{
			public Point16 Anchor;
			public RegionSnapshot Before;
			public RegionSnapshot After;
		}

		private const int MaxHistory = 50;

		public static ModKeybind UndoKeybind;
		public static ModKeybind RedoKeybind;

		private static readonly List<Change> _undoStack = new();
		private static readonly List<Change> _redoStack = new();

		public override void Load()
		{
			UndoKeybind = KeybindLoader.RegisterKeybind(Mod, "Undo", "Z");
			RedoKeybind = KeybindLoader.RegisterKeybind(Mod, "Redo", "Y");
		}

		public override void Unload()
		{
			UndoKeybind = null;
			RedoKeybind = null;
			_undoStack.Clear();
			_redoStack.Clear();
		}

		// Call after a paste/erase completes with a before/after snapshot of the region it touched.
		public static void Record(Point16 anchor, RegionSnapshot before, RegionSnapshot after)
		{
			_undoStack.Add(new Change { Anchor = anchor, Before = before, After = after });
			if (_undoStack.Count > MaxHistory)
				_undoStack.RemoveAt(0);

			_redoStack.Clear();
		}

		public static void Undo()
		{
			if (_undoStack.Count == 0)
				return;

			Change change = _undoStack[^1];
			_undoStack.RemoveAt(_undoStack.Count - 1);
			change.Before.Apply(change.Anchor);
			_redoStack.Add(change);
		}

		public static void Redo()
		{
			if (_redoStack.Count == 0)
				return;

			Change change = _redoStack[^1];
			_redoStack.RemoveAt(_redoStack.Count - 1);
			change.After.Apply(change.Anchor);
			_undoStack.Add(change);
		}
	}
}
