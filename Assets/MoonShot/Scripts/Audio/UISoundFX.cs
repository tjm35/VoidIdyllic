using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Moonshot.Audio
{
	public class UISoundFX : MonoBehaviour
	{
		public InputActionReference m_navigateAction;
		public InputActionReference m_submitAction;

		public SoundFXRef m_confirmFX;
		public SoundFXRef m_moveCursorFX;
		public SoundFXRef m_cantMoveCursorFX;

		void OnEnable()
		{
			m_navigateAction.action.started += OnNavigatePressed;
			m_submitAction.action.started += OnSubmitPressed;
		}

		void OnDisable()
		{
			m_navigateAction.action.started -= OnNavigatePressed;
			m_submitAction.action.started -= OnSubmitPressed;
		}

		private void OnNavigatePressed(InputAction.CallbackContext i_context)
		{
			m_moveAttemptQueued = true;
		}

		private void OnSubmitPressed(InputAction.CallbackContext i_context)
		{
			m_confirmFX.PlaySound();
		}

		// Update is called once per frame
		void Update()
		{
			m_framesSinceSelectionChanged++;
			GameObject selected = EventSystem.current.currentSelectedGameObject;
			if (selected != m_lastSelected)
			{
				//Debug.Log($"Selection change from {(m_lastSelected ? m_lastSelected.name : 0.ToString())} to {(selected ? selected.name : 0.ToString())}");
				m_framesSinceSelectionChanged = 0;
				m_lastSelected = selected;
			}

			if (m_moveAttemptQueued)
			{
				//Debug.Log($"Cursor moved ({m_framesSinceSelectionChanged} frames since last selection change)");
				if (m_framesSinceSelectionChanged < 2)
				{
					m_moveCursorFX.PlaySound();
				}
				else
				{
					m_cantMoveCursorFX.PlaySound();
				}
				m_moveAttemptQueued = false;
			}
		}

		private GameObject m_lastSelected = null;
		private int m_framesSinceSelectionChanged = 99;
		private bool m_moveAttemptQueued = false;
	}
}