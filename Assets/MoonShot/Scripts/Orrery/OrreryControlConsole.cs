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
				}
				else
				{
					OrreryTimeSource.Global.m_playRate = 1.0f;
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
	}
}