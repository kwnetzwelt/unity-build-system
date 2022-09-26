using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace UBS
{
    [Serializable]
    public class UBSStepListWalker
    {
        [field: FormerlySerializedAs("mIndex")]
        [field: SerializeField]
        public int Index { get; private set; } = 0;

        [field: FormerlySerializedAs("mSteps")]
        [field: SerializeField]
        public List<BuildStep> Steps { get; private set; }


        IBuildStepProvider _currentStep;
        private BuildStepProviderEntry _currentStepEntry;

        [field: FormerlySerializedAs("mConfiguration")]
        [field: SerializeField]
        public BuildConfiguration Configuration { get; private set; }

        public UBSStepListWalker()
        {

        }

        public void Init ( List<BuildStep> steps, BuildConfiguration configuration)
        {
            _currentStep = null;
            Index = 0;
            Steps = steps;
            Configuration = configuration;
        }
        public void Clear()
        {
            if (_currentStep != null)
            {
                _currentStep.BuildStepUpdate(); // call update one last time, to let the step know we are done. 
            }

            _currentStep = null;
            Index = 0;
            Steps = null;
            Configuration = null;
        }

        public void MoveNext()
        {
            if(_currentStep == null || _currentStep.IsBuildStepDone())
            {
                NextStep();
            }else
            {
                _currentStep.BuildStepUpdate();
            }
        }
		
        void NextStep()
        {
            if(Steps == null)
                return;
            if(IsDone())
            {
                return;
            }

            if(_currentStep != null)
                Index++;
            
            if(Index >= Steps.Count)
                return;
            
            // skips disabled steps
            while (Index < Steps.Count)
            {
                if (Steps[Index].Enabled)
                    break;
                Index++;
            }
            if(Index > Steps.Count-1)
                return;
            
            bool result = Steps[Index].TryInferType();

            if (!result)
                return;
            if (Steps [Index].StepType != null) 
            {
                _currentStep = Activator.CreateInstance(Steps[Index].StepType) as IBuildStepProvider;
                _currentStepEntry = new BuildStepProviderEntry(Steps[Index].StepType);
                Configuration.SetParams( Steps[Index].Parameters );
            }
            else 
            {
                _currentStep = new SkipBuildStepEntry();
            }

            if (_currentStep == null)
            {
                return;
            }
			
            Debug.Log("Starting Build Step: " + _currentStep.GetType().ToString());
            _currentStep.BuildStepStart(Configuration);			
        }

        public bool IsDone()
        {
            if(Steps != null)
                return Index == Steps.Count;
            else
                return false;
        }

        public float Count {
            get
            {
                if(Steps == null || Steps.Count == 0)
                    return 0;
                return (float)Steps.Count;
            }
        }

        public float Progress
        {
            get
            {
                if(Index == 0 && Count == 0)
                    return 1;
                return Index / Count;
            }
        }
        public string CurrentStep
        {
            get
            {
                if(_currentStepEntry == null || Steps == null || Index >= Steps.Count)
                    return "N/A";
                return _currentStepEntry.ToMenuPath();
            }
        }
    }
}