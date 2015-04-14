using System;
namespace UBS
{
	/// <summary>
	/// Implement this interface in a custom class. The build system will pick up your class and it will become available as a build step. 
	/// 
	/// The <see cref="IBuildStepProvider::BuildStepStart"/> method will be called once your custom class is run as a build step by the system. 
	/// The pConfiguration (<see cref="BuildConfiguration"/>) parameter contains all the information of the build currently happening. 
	/// You can access parameters set in the specific step instance through the pConfiguration parameter. 
	/// 
	/// Once your step instance has been setup, the build system will continuously call your implementation of <see cref="IBuildStepProvider::BuildStepUpdate"/> until your
	/// <see cref="IBuildStepProvider::IsBuildStepDone"/> implementation returns true. 
	/// </summary>
	public interface IBuildStepProvider
	{

		/// <summary>
		/// Called by the Buildsystem if your build step Provider is requested to start its build step
		/// </summary>
		/// <param name="pConfiguration">configuration.</param>
		void BuildStepStart(BuildConfiguration pConfiguration);
		/// <summary>
		/// Called as an update method every "frame" to allow continuous processing of async operations
		/// </summary>
		void BuildStepUpdate();
		/// <summary>
		/// Determines whether this instance is done.
		/// </summary>
		/// <returns><c>true</c> if this instance is done; otherwise, <c>false</c>.</returns>
		bool IsBuildStepDone();
		

	}
}

