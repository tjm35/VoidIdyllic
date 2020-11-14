using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Tutorial
{
	public class TutorialHelper : MonoBehaviour
	{
		public PlayMakerFSM m_fsm;

		public static TutorialHelper Instance;

		public static void SetBool(string i_name, bool i_value = true)
		{
			Instance.Set(i_name, i_value);
		}

		public void Set(string i_name, bool i_value)
		{
			m_fsm.Fsm.GetFsmBool(i_name).Value = i_value;
		}

		private void OnEnable()
		{
			Instance = this;
		}

		private void OnDisable()
		{
			Instance = null;
		}
	}
}