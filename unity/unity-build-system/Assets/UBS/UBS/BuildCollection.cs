using System;
using System.Collections.Generic;
using UnityEngine;

namespace UBS
{
	public class BuildCollection : ScriptableObject
	{
		public BuildCollection ()
		{
		}

		public List<BuildProcess> mProcesses = new List<BuildProcess>();

	}
}

