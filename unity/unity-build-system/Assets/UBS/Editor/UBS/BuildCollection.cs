using System;
using System.Collections.Generic;
using UnityEngine;

namespace UBS
{
	[Serializable]
	public class BuildCollection : ScriptableObject
	{
		public BuildCollection ()
		{
		}

		public List<BuildProcess> mProcesses = new List<BuildProcess>();

	}
}

