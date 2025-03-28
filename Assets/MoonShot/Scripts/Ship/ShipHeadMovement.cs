﻿using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Ship
{
	public class ShipHeadMovement : MonoBehaviour
	{
		public Vector3 m_maxAngles = new Vector3(20.0f, 20.0f, 30.0f);
		public float m_bankPropWhenTurning = 0.2f;
		public float m_turnBlendRate = 0.01f;
		public Vector3 m_maxOffsets = new Vector3(0.1f, 0.1f, 0.1f);

		private void Update()
		{
			var targetAngles = Vector3.zero;
			var targetOffset = Vector3.zero;
			var controls = transform.GetComponentInAncestors<ShipControls>();
			if (controls)
			{
				Vector3 clampedLook = controls.Look;
				clampedLook.x = Mathf.Clamp(clampedLook.x, -1.0f, 1.0f);
				clampedLook.y = Mathf.Clamp(clampedLook.y, -1.0f, 1.0f);
				clampedLook.z = Mathf.Clamp(clampedLook.z, -1.0f, 1.0f);
				targetAngles = Vector3.Scale(clampedLook, new Vector3(-1.0f, 1.0f, -1.0f));
				targetOffset = -1.0f * Vector3.Scale(controls.Move, m_maxOffsets);
			}
			targetAngles.z += m_bankPropWhenTurning * targetAngles.y;
			targetAngles.Scale(m_maxAngles);

			// TODO: Better blending
			m_currentAngles = Vector3.Lerp(m_currentAngles, targetAngles, m_turnBlendRate);

			m_currentAngles.x = Mathf.Clamp(m_currentAngles.x, -m_maxAngles.x, m_maxAngles.x);
			m_currentAngles.y = Mathf.Clamp(m_currentAngles.y, -m_maxAngles.y, m_maxAngles.y);
			m_currentAngles.z = Mathf.Clamp(m_currentAngles.z, -m_maxAngles.z, m_maxAngles.z);

			transform.localEulerAngles = m_currentAngles;

			transform.localPosition = Vector3.Lerp(transform.localPosition, targetOffset, m_turnBlendRate);
		}

		private Vector3 m_currentAngles = Vector3.zero;
	}
}