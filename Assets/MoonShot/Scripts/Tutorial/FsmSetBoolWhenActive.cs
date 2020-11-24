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
		public float m_minLingerTime = 0.5f;

		private void OnEnable()
		{
			if (m_minLingerTime == 0.0f)
			{
				TutorialHelper.SetBool(m_variableName, true);
				m_hasSet = true;
			}
		}

		private void Update()
		{
			if (!m_hasSet)
			{
				m_lingerTime += Time.deltaTime;
				if (m_lingerTime > m_minLingerTime)
				{
					TutorialHelper.SetBool(m_variableName, true);
					m_hasSet = true;
				}
			}
		}

		private void OnDisable()
		{
			if (m_unsetOnDisable)
			{
				TutorialHelper.SetBool(m_variableName, false);
			}
			m_lingerTime = 0.0f;
			m_hasSet = false;
		}

		private float m_lingerTime = 0.0f;
		private bool m_hasSet = false;
	}
}