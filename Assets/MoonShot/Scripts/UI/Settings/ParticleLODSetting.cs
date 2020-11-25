using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.UI
{
	[RequireComponent(typeof(Slider))]
	public class ParticleLODSetting : MonoBehaviour
	{
		public int m_maxQuality = 4;

		public float ParticleQuality
		{
			get { return (float)(m_maxQuality - ParticleLOD); }
			set { ParticleLOD = Mathf.RoundToInt(m_maxQuality - value); }
		}

		public void Start()
		{
			GetComponent<Slider>().maxValue = m_maxQuality;
			GetComponent<Slider>().value = ParticleQuality;
		}

		public static int ParticleLOD = 1;
	}
}