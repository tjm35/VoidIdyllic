using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	public class OrreryTimeSource : MonoBehaviour
	{
		public bool m_advancing = true;
		public float TimeElapsed = 0.0f;
		public bool m_isGlobal = true;

		public static OrreryTimeSource Global;

		private void Start()
		{
			if (m_isGlobal)
			{
				Debug.Assert(Global == null);
				Global = this;
			}
		}

		private void OnDestroy()
		{
			if (Global == this)
			{
				Global = null;
			}
		}

		void Update()
		{
			if (m_advancing)
			{
				TimeElapsed += Time.deltaTime;
			}
		}
	}
}