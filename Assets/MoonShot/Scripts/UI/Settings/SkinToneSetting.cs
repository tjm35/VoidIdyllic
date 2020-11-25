using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.UI
{
	public class SkinToneSetting : MonoBehaviour
	{
		[Serializable]
		public class ToneSetup
		{
			[ColorUsage(false, false)]
			public Color m_primaryColor = Color.black;
			[ColorUsage(false, true)]
			public Color m_emissiveColor = Color.black;
			public float m_metallic = 0.0f;
			public float m_smoothness = 0.5f;
		}

		public ToneSetup[] m_tones;

		public float ToneIndex
		{
			set
			{
				int index = Mathf.Clamp(Mathf.RoundToInt(value), 0, m_tones.Length - 1);
				Tone = m_tones[index];
			}
		}

		public static ToneSetup Tone { get; private set; }

		public void Start()
		{
			GetComponent<Slider>().maxValue = m_tones.Length - 1;
		}
	}
}