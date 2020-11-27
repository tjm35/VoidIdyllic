using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using HutongGames.PlayMaker;

namespace Totality.PlayMakerIntegration.Mixer
{
	[ActionCategory("InputSystem")]
	public class SetMixerParam : FsmStateAction
	{
		[RequiredField]
		public AudioMixer m_mixer;
		[RequiredField]
		public FsmString m_paramName;

		public FsmFloat m_value;
		public bool m_everyFrame;

		public override void Reset()
		{
			m_mixer = null;
			m_paramName = "";
			m_value = 0.0f;
			m_everyFrame = false;
		}

		public override void OnEnter()
		{
			m_mixer.SetFloat(m_paramName.Value, m_value.Value);
			if (!m_everyFrame)
			{
				Finish();
			}
		}

		public override void OnUpdate()
		{
			m_mixer.SetFloat(m_paramName.Value, m_value.Value);
		}

	}
}