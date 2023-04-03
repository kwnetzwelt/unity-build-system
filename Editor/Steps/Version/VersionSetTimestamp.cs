namespace UBS.Version
{
	public class VersionSetTimestamp
	{
		[BuildStepDescriptionAttribute(
			"Sets the version timestamp to current data and time")]
		public class SetTimeStamp: IBuildStepProvider
		{
			#region IBuildStepProvider implementation

			public void BuildStepStart(BuildConfiguration configuration)
			{
				var collection = configuration.GetCurrentBuildCollection();
				collection.version.buildTimestamp = System.DateTime.UtcNow.ToString("yyyy MMMM dd - HH:mm");
				collection.SaveVersion(configuration.ToggleValue);
			}

			public void BuildStepUpdate()
			{

			}

			public bool IsBuildStepDone()
			{
				return true;
			}

			#endregion
		}
	}
}