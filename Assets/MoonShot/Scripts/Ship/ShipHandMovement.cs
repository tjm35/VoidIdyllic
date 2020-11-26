using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Ship
{
	[RequireComponent(typeof(Animator))]
	public class ShipHandMovement : MonoBehaviour
	{
		public float m_turnBlendRate = 0.1f;

		// Start is called before the first frame update
		void Start()
		{
			m_animator = GetComponent<Animator>();
			m_leftHorizID = Animator.StringToHash("LeftHoriz");
			m_leftVertID = Animator.StringToHash("LeftVert");
			m_rightHorizID = Animator.StringToHash("RightHoriz");
			m_rightVertID = Animator.StringToHash("RightVert");
		}

		// Update is called once per frame
		void Update()
		{
			Vector2 leftTarget = Vector2.zero;
			Vector2 rightTarget = Vector2.zero;
			var controls = transform.GetComponentInAncestors<ShipControls>();
			if (controls)
			{
				Vector3 clampedLook = controls.Look;
				rightTarget.x = Mathf.Clamp(clampedLook.y, -1.0f, 1.0f);
				rightTarget.y = Mathf.Clamp(clampedLook.x, -1.0f, 1.0f);
				leftTarget.x = controls.Move.x;
				leftTarget.y = controls.Move.z;
			}

			m_leftPos = Vector2.Lerp(m_leftPos, leftTarget, m_turnBlendRate);
			m_rightPos = Vector2.Lerp(m_rightPos, rightTarget, m_turnBlendRate);

			m_animator.SetFloat(m_leftHorizID, m_leftPos.x);
			m_animator.SetFloat(m_leftVertID, m_leftPos.y);
			m_animator.SetFloat(m_rightHorizID, m_rightPos.x);
			m_animator.SetFloat(m_rightVertID, m_rightPos.y);
		}

		private Vector2 m_leftPos = Vector2.zero;
		private Vector2 m_rightPos = Vector2.zero;

		private Animator m_animator;
		private int m_leftHorizID;
		private int m_leftVertID;
		private int m_rightHorizID;
		private int m_rightVertID;
	}
}