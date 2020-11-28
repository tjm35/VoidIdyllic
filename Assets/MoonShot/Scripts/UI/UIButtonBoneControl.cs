using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Moonshot.UI
{
	public class UIButtonBoneControl : MonoBehaviour
	{
		public InputActionReference m_navigateAction;
		public InputActionReference m_submitAction;
		public float m_xMaxAngle = 2.0f;
		public float m_yMaxAngle = 2.0f;
		public float m_zMaxAngle = 2.0f;
		public float m_moveAcceptAngle = 0.1f;
		public float m_angleBlendRate = 0.1f;

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
			m_moveAttemptDirection = i_context.ReadValue<Vector2>().normalized;
		}

		private void OnSubmitPressed(InputAction.CallbackContext i_context)
		{
			m_submitQueued = true;
		}

		// Update is called once per frame
		private void Update()
		{
			var targetEuler = GetTargetEuler();

			m_currentEuler = Vector3.Lerp(m_currentEuler, targetEuler, m_angleBlendRate);
			DetectCompletion(m_currentEuler, targetEuler);
			transform.localEulerAngles = m_currentEuler;
		}

		private Vector3 GetTargetEuler()
		{
			var target = Vector3.zero;
			if (m_moveAttemptQueued)
			{
				target.x += m_xMaxAngle * m_moveAttemptDirection.y;
				target.y += m_yMaxAngle * m_moveAttemptDirection.x;
			}
			if (m_submitQueued)
			{
				target.z += m_zMaxAngle;
			}
			return target;
		}

		private void DetectCompletion(Vector3 current, Vector3 target)
		{
			if (Mathf.Abs(current.x - target.x) < m_moveAcceptAngle && Mathf.Abs(current.y - target.y) < m_moveAcceptAngle)
			{
				m_moveAttemptQueued = false;
			}
			if (Mathf.Abs(current.z - target.z) < m_moveAcceptAngle)
			{
				m_submitQueued = false;
			}
		}

		private bool m_submitQueued = false;
		private bool m_moveAttemptQueued = false;
		private Vector2 m_moveAttemptDirection = Vector2.zero;
		private Vector3 m_currentEuler = Vector3.zero;
	}
}