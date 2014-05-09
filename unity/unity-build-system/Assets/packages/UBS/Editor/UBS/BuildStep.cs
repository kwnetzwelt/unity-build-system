using UnityEngine;
using System.Collections;
using System;

namespace UBS
{
	[Serializable]
	public class BuildStep {

		public BuildStep()
		{

		}

		public BuildStep(BuildStep pOther)
		{
			mType = pOther.mType;
			mTypeName = pOther.mTypeName;
			mParams = pOther.mParams;
		}
		public string mParams = "";
		public string mTypeName = "";

		public System.Type mType;

		public void InferType()
		{
			mType = System.Type.GetType( mTypeName );
		}

		public void SetType(System.Type pType)
		{

			mType = pType;
			if(pType == null)
				mTypeName = "";
			else
				mTypeName = mType.ToString();
		}

	}
}