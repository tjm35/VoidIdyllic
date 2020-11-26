using OVR;
using Moonshot.Photos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	public class OrreryControlConsole : MonoBehaviour
	{
		public enum ControlType
		{
			Pause,
			FFwd,
			Rwnd,
		}

		public ControlType m_controlType;
		public Renderer m_iconRenderer;
		public SoundFXRef m_ruinFX;
		public float m_audioFadeTime = 0.25f;

		public void OnEnteredViewfinder(PointOfInterest i_poi)
		{
			//Debug.Log($"{gameObject.name} OnEnteredViewfinder ({m_controlType})");
			if (m_controlType == ControlType.FFwd)
			{
				OrreryTimeSource.Global.m_playRate = 8.0f;
			}
			if (m_controlType == ControlType.Rwnd)
			{
				OrreryTimeSource.Global.m_playRate = -8.0f;
				m_fxID = m_ruinFX.PlaySound();
			}
			if (m_controlType != ControlType.Pause)
			{
				if (m_fxID == -1)
				{
					m_fxID = m_ruinFX.PlaySound();
				}
				AudioManager.FadeInSound(m_fxID, m_audioFadeTime);
			}
			m_inViewfinder = true;
		}

		public void OnExitedViewfinder(PointOfInterest i_poi)
		{
			//Debug.Log($"{gameObject.name} OnExitedViewfinder ({m_controlType})");
			if (m_controlType == ControlType.FFwd || m_controlType == ControlType.Rwnd)
			{
				if (OrreryTimeSource.Global.m_gameplayPaused)
				{
					OrreryTimeSource.Global.m_playRate = 0.0f;
				}
				else
				{
					OrreryTimeSource.Global.m_playRate = 1.0f;
				}
			}
			if (m_controlType != ControlType.Pause)
			{
				if (m_fxID != -1)
				{
					AudioManager.FadeOutSound(m_fxID, m_audioFadeTime);
				}
			}
			m_inViewfinder = false;
		}

		public void OnPhotoTaken(PointOfInterest i_poi)
		{
			//Debug.Log($"{gameObject.name} OnPhotoTaken ({m_controlType})");
			if (m_controlType == ControlType.Pause)
			{
				OrreryTimeSource.Global.m_gameplayPaused = !OrreryTimeSource.Global.m_gameplayPaused;
				if (OrreryTimeSource.Global.m_gameplayPaused)
				{
					OrreryTimeSource.Global.m_playRate = 0.0f;
					m_ruinFX.PlaySound();
				}
				else
				{
					OrreryTimeSource.Global.m_playRate = 1.0f;
					m_ruinFX.PlaySound();
				}
				UpdateIcon();
			}
		}

		private void Start()
		{
			UpdateIcon();
		}

		private void UpdateIcon()
		{
			if (m_controlType == ControlType.Pause && m_iconRenderer)
			{
				m_iconRenderer.material.SetInt("_IsActive", OrreryTimeSource.Global.m_gameplayPaused ? 1 : 0);
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
	}
}