using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Moonshot.Photos
{
	public class GoalUIEntry : MonoBehaviour
	{
		public TMP_Text m_text;
		public Toggle m_toggle;

		public Goal Goal
		{
			get { return m_goal; }
			set
			{
				m_goal = value;
				m_text.text = m_goal.m_description;
			}
		}

		private void Update()
		{
			if (m_goal)
			{
				m_toggle.isOn = PhotoSystem.Instance.IsGoalComplete(m_goal);
			}
		}

		private Goal m_goal;
	}
}