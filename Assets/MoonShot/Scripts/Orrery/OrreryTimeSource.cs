using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	public class OrreryTimeSource : MonoBehaviour
	{
		public bool m_advancing = true;
		public float TimeElapsed { get; private set; } = 0.0f;

		void Update()
		{
			if (m_advancing)
			{
				TimeElapsed += Time.deltaTime;
			}
		}
	}
}