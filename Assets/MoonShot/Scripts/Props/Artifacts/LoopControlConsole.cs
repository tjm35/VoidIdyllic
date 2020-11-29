using OVR;
using Moonshot.Photos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class LoopControlConsole : MonoBehaviour
	{
		public SoundFXRef m_ruinFX;
		public float m_audioFadeTime = 0.25f;
		public float m_turnSpeed = 30.0f;
		public Transform m_car;

		public void OnEnteredViewfinder(PointOfInterest i_poi)
		{
			if (m_fxID == -1)
			{
				m_fxID = m_ruinFX.PlaySound();
			}
			AudioManager.FadeInSound(m_fxID, m_audioFadeTime);
			m_inViewfinder = true;
		}

		public void OnExitedViewfinder(PointOfInterest i_poi)
		{
			if (m_fxID != -1)
			{
				AudioManager.FadeOutSound(m_fxID, m_audioFadeTime);
			}
			m_inViewfinder = false;
		}

		private void Start()
		{
			m_highResProp = transform.GetComponentInAncestors<HighResProp>();
		}

		private void FixedUpdate()
		{
			if (m_inViewfinder && m_highResProp)
			{
				float zRot = m_car.transform.localEulerAngles.z;
				zRot -= m_turnSpeed * Time.fixedDeltaTime;
				m_car.localEulerAngles = zRot * Vector3.forward;
				if (m_highResProp.OrreryProp)
				{
					m_highResProp.OrreryProp.transform.localEulerAngles = zRot * Vector3.forward;
				}
			}
		}

		private void OnDestroy()
		{
			if (m_inViewfinder)
			{
				OnExitedViewfinder(GetComponent<PointOfInterest>());
			}
		}

		private bool m_inViewfinder = false;
		private int m_fxID = -1;
		private HighResProp m_highResProp;
	}
}