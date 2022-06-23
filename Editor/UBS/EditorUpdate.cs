using System;
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace UBS
{
	internal class EditorUpdate
	{
		class Entry
		{
            public Func<bool> IsDone { get; }
            public Action OnDone { get; }

            public Entry (Func<bool> isDone, Action onDone)
			{
				this.IsDone = isDone;
				this.OnDone = onDone;
			}
		}

		static List<Entry> _entries = new List<Entry>();

		public static void Run(Func<bool> isDone , Action onDone)
		{
			if(_entries.Count == 0)
			{
				EditorApplication.update += OnEditorUpdate;
			}
			_entries.Add(new Entry(isDone, onDone));

		}

		static void OnEditorUpdate()
		{
			if(_entries.Count == 0)
			{
				EditorApplication.update -= OnEditorUpdate;
			}
			else
			{
				var e = _entries[0];
				if(e.IsDone())
				{
					_entries.RemoveAt(0);
					e.OnDone();
				}
			}
		}
	}
}

