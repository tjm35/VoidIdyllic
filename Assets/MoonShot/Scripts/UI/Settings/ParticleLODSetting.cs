using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.UI
{
	public class ParticleLODSetting : MonoBehaviour
	{
		public int m_maxQuality = 4;

		public float ParticleQuality
		{
			get { return (float)(m_maxQuality - ParticleLOD); }
			set { ParticleLOD = Mathf.RoundToInt(m_maxQuality - value); }
		}

		public static int ParticleLOD = 0;
	}
}