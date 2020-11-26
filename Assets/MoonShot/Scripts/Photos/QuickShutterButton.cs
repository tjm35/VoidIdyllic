using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Moonshot.Photos
{
	public class QuickShutterButton : MonoBehaviour
	{
		public PhotoSystem m_photoSystem;
		public Camera m_camera;
		public InputActionReference m_shutterButton;
		public UnityEvent m_onShutterClicked = new UnityEvent();
		public Animation m_shutterAnimation;
		public float m_minPhotoInterval = 0.5f;

		void Start()
		{
			InputAction action = m_shutterButton?.action;
			if (action != null)
			{
				action.performed += TakePhoto;
			}
		}

		void OnDestroy()
		{
			InputAction action = m_shutterButton?.action;
			if (action != null)
			{
				action.performed -= TakePhoto;
			}
		}

		void TakePhoto(InputAction.CallbackContext i_context)
		{
			if (isActiveAndEnabled)
			{
				m_photoRequested = true;
			}
		}

		private void Update()
		{
			m_photoElapsedTime += Time.deltaTime;

			if (m_photoRequested && m_photoElapsedTime > m_minPhotoInterval)
			{
				m_photoSystem.TakePhoto(m_camera);
				m_onShutterClicked.Invoke();
				if (m_shutterAnimation)
				{
					m_shutterAnimation.Stop();
					m_shutterAnimation.Play();
				}

				m_photoRequested = false;
				m_photoElapsedTime = 0.0f;
			}
		}

		private bool m_photoRequested = false;
		private float m_photoElapsedTime = 999.0f;
	}
}