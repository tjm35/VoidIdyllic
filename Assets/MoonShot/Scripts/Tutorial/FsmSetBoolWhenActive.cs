using HutongGames.PlayMaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Tutorial
{
	public class FsmSetBoolWhenActive : MonoBehaviour
	{
		public PlayMakerFSM m_stateMachine;
		public string m_variableName;
		public bool m_unsetOnDisable;

		private void OnEnable()
		{
			TutorialHelper.SetBool(m_variableName, true);
		}

		private void OnDisable()
		{
			if (m_unsetOnDisable)
			{
				TutorialHelper.SetBool(m_variableName, false);
			}
		}

	}
}