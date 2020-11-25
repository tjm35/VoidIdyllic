using OVR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moonshot.Audio
{
	public class LocomotionAudio : MonoBehaviour
	{
		public SoundFXRef m_jetpackBoost;
		public SoundFXRef m_jetpackHover;
		public float m_jetpackBoostTime = 3.0f;
		public float m_jetpackHoverCrossFadeTime = 1.0f;
		public float m_startHoverFadeTime = 0.5f;
		public float m_stopFadeTime = 0.5f;
		public Transform m_jetpackLocation;

		[Serializable]
		public class FootfallSound
		{
			public PhysicMaterial m_material;
			public SoundFXRef m_sound;
		}

		public List<FootfallSound> m_footfalls;

		public void StartJetpack(bool i_withBoost)
		{
			if (!m_jetpackOn)
			{
				m_jetpackOn = true;
				m_jetpackTime = 0.0f;
				if (i_withBoost)
				{
					m_boostPlayingIndex = m_jetpackBoost.PlaySound();
					if (m_jetpackLocation)
					{
						AudioManager.AttachSoundToParent(m_boostPlayingIndex, m_jetpackLocation);
					}
				}
				else
				{
					m_hoverPlayingIndex = m_jetpackHover.PlaySound();
					AudioManager.FadeInSound(m_hoverPlayingIndex, m_startHoverFadeTime);
					if (m_jetpackLocation)
					{
						AudioManager.AttachSoundToParent(m_hoverPlayingIndex, m_jetpackLocation);
					}
				}
			}
		}

		public void StopJetpack()
		{
			m_jetpackOn = false;
		}

		public void DoFootfall(PhysicMaterial i_material, float i_volume)
		{
			var sound = m_footfalls.First(s => s.m_material == i_material);
			if (sound == null)
			{
				sound = m_footfalls.First(s => s.m_material == null);
			}
			if (sound != null)
			{
				sound.m_sound.PlaySoundAt(transform.position, 0, i_volume);
			}
		}

		// Update is called once per frame
		private void Update()
		{
			if (m_jetpackOn)
			{
				m_jetpackTime += Time.unscaledDeltaTime;
				if (m_jetpackTime > m_jetpackBoostTime && m_boostPlayingIndex != -1)
				{
					AudioManager.FadeOutSound(m_boostPlayingIndex, m_jetpackHoverCrossFadeTime);
					m_boostPlayingIndex = -1;
					if (m_hoverPlayingIndex == -1)
					{
						m_hoverPlayingIndex = m_jetpackHover.PlaySound();
						AudioManager.FadeInSound(m_hoverPlayingIndex, m_jetpackHoverCrossFadeTime);
						if (m_jetpackLocation)
						{
							AudioManager.AttachSoundToParent(m_hoverPlayingIndex, m_jetpackLocation);
						}
					}
				}
			}
			else
			{
				if (m_boostPlayingIndex != -1)
				{
					AudioManager.FadeOutSound(m_boostPlayingIndex, m_stopFadeTime);
					m_boostPlayingIndex = -1;
				}
				if (m_hoverPlayingIndex != -1)
				{
					AudioManager.FadeOutSound(m_hoverPlayingIndex, m_stopFadeTime);
					m_hoverPlayingIndex = -1;
				}
			}
		}

		private bool m_jetpackOn = false;
		private float m_jetpackTime = 0.0f;
		private int m_boostPlayingIndex = -1;
		private int m_hoverPlayingIndex = -1;
	}
}