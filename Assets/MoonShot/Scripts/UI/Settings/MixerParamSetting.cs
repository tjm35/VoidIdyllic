using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Moonshot.UI
{
	public class MixerParamSetting : MonoBehaviour
	{
		public AudioMixer m_mixer;
		public string m_paramName;

		public float SettingPercentToDB
		{
			set { Setting = PercentToDB(value); }
		}

		public float Setting
		{
			set { m_mixer.SetFloat(m_paramName, value); }
		}

		private float PercentToDB(float m_percent)
		{
			return 10.0f * Mathf.Log(Mathf.Max(0.01f * m_percent, 0.00001f));
		}
	}
}