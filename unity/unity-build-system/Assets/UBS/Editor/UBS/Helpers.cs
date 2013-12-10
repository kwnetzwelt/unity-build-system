using System;
using System.Collections.Generic;
using System.Linq;

namespace UBS
{
	public class Helpers
	{


		public static List<System.Type> FindClassesImplementingInterface( System.Type pInterface)
		{
			List<System.Type> mOutList = new List<System.Type>();
			foreach(var assembly in AppDomain.CurrentDomain.GetAssemblies())
			{

				foreach(var t in assembly.GetTypes())
				{

					if(t.GetInterfaces().Contains( pInterface ))
					{
						mOutList.Add(t);
					}

				}

			}
			return mOutList;
		}

	}
}

