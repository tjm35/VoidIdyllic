using HutongGames.PlayMaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Tutorial
{
	[ActionCategory("Tutorial")]
	public class TutorialCheck : FsmStateAction
	{
		[Serializable]
		public class Condition
		{
			[RequiredField]
			[UIHint(UIHint.Variable),Readonly]
			public FsmBool variable;

			[RequiredField]
			[Readonly]
			public bool required; 
		}

		public Condition[] conditions;

		public override void Reset()
		{
			conditions = null;
		}

		public override void OnEnter()
		{
			promptActive = Fsm.GetFsmBool("PromptActive");
			targetFSM = Fsm.GetFsmObject("PromptFsm").Value as PlayMakerFSM;
		}

		public override void OnUpdate()
		{
			if (promptActive.Value)
			{
				// A prompt is already active, so we do nothing.
			}
			else
			{
				bool conditionsMet = true;
				for (int i = 0; i < conditions.Length; ++i)
				{
					if (conditions[i].variable.Value != conditions[i].required)
					{
						conditionsMet = false;
					}
				}
				if (conditionsMet)
				{
					targetFSM.SetState(TargetState);
					promptActive.Value = true;
				}
			}
		}

		private string TargetState => Name;

		private FsmBool promptActive;
		private PlayMakerFSM targetFSM;
	}
}