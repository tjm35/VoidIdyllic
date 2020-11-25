using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Audio
{
	public class PlaySoundFX : MonoBehaviour
	{
		public SoundFXRef m_soundFX;
		public bool m_locational = false;

		public void Play()
		{
			if (m_locational)
			{
				m_soundFX.AttachToParent(transform);
			}
			m_soundFX.PlaySound();
		}
	}
}