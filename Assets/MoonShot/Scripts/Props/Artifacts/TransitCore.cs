using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class TransitCore : MonoBehaviour
	{
		public SoundFXRef m_ruinFX;
		public GameObject[] m_chunkObjects;
		public float m_activeTime = 3.0f;
		public float m_audioFadeTime = 0.1f;

		public void Activate(int i_dest)
		{
			for (int i = 0; i < m_chunkObjects.Length; ++i)
			{
				m_chunkObjects[i].SetActive(i == i_dest);
			}
			if (m_fxID != -1)
			{
				AudioManager.FadeOutSound(m_fxID, m_audioFadeTime);
			}
			m_fxID = m_ruinFX.PlaySound();
			m_activeTimer = 0.0f;
			m_activeObject = i_dest;
			m_active = true;
		}

		public bool IsActive(int i_dest)
		{
			return m_activeObject == i_dest;
		}

		private void Update()
		{
			if (m_active)
			{
				m_activeTimer += Time.deltaTime;
				if (m_activeTimer > m_activeTime)
				{
					for (int i = 0; i < m_chunkObjects.Length; ++i)
					{
						m_chunkObjects[i].SetActive(false);
					}
					if (m_fxID != -1)
					{
						AudioManager.FadeOutSound(m_fxID, m_audioFadeTime);
					}
					m_active = false;
					m_activeObject = -1;

					// TODO: Teleport player if they are in zone.
				}
			}
		}

		private bool m_active = false;
		private int m_fxID = -1;
		private float m_activeTimer = 0.0f;
		private int m_activeObject = -1;
	}
}