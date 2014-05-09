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
			public Func<bool> mIsDone;
			public Action mOnDone;
			public Entry (Func<bool> pIsDone, Action pOnDone)
			{
				mIsDone = pIsDone;
				mOnDone = pOnDone;
			}
		}

		static List<Entry> mEntries = new List<Entry>();

		public static void Run(Func<bool> pIsDone , Action pOnDone)
		{
			if(mEntries.Count == 0)
			{
				EditorApplication.update += OnEditorUpdate;
			}
			mEntries.Add(new Entry(pIsDone, pOnDone));

		}

		static void OnEditorUpdate()
		{
			if(mEntries.Count == 0)
			{
				EditorApplication.update -= OnEditorUpdate;
			}
			else
			{
				var e = mEntries[0];
				if(e.mIsDone())
				{
					mEntries.RemoveAt(0);
					e.mOnDone();
				}
			}
		}
	}
}

