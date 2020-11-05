using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moonshot.Photos
{
	public class QuickShutterButton : MonoBehaviour
	{
		public PhotoSystem m_photoSystem;
		public Camera m_camera;
		public InputActionReference m_shutterButton;

		// Update is called once per frame
		void Start()
		{
			InputAction action = m_shutterButton?.action;
			if (action != null)
			{
				action.performed += TakePhoto;
			}
		}

		void TakePhoto(InputAction.CallbackContext i_context)
		{ 
			m_photoSystem.TakePhoto(m_camera);
		}
	}
}