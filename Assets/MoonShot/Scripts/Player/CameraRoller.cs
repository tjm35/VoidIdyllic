using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moonshot.Player
{
	public class CameraRoller : MonoBehaviour
	{
		public float m_rollSpeed = 90.0f;
		public InputActionReference RollAction;
		public InputActionReference UIRollAction;

		void OnEnable()
		{
			transform.localRotation = Quaternion.identity;
		}

		// Update is called once per frame
		void Update()
		{
			float rollControl = RollAction.action.ReadValue<float>() + UIRollAction.action.ReadValue<float>();

			float currentAngle = transform.localRotation.eulerAngles.z;
			float newAngle = Mathf.Clamp(WrapAngleDegrees(180.0f + currentAngle - rollControl * m_rollSpeed * Time.unscaledDeltaTime) - 180.0f, -90.0f, 90.0f);

			transform.localRotation = Quaternion.Euler(0.0f, 0.0f, newAngle);
		}

		private float WrapAngleDegrees(float angle)
		{
			float turns = angle / 360.0f;
			float localTurns = turns - Mathf.Floor(turns);
			return localTurns * 360.0f;
		}

	}
}