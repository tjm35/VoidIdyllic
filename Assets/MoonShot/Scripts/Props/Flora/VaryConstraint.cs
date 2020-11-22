using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class VaryConstraint : MonoBehaviour
	{
		public Quaternion m_baseRotation = Quaternion.identity;
		[Button]
		public void CopyBaseFromCurrent()
		{
			m_baseRotation = transform.localRotation;
		}
		public bool m_useCustomAngleRange = false;
		public Vector3 m_customAngleRange = Vector3.zero;
		public float m_forcedFacingWeight = 0.0f;
		public Vector3 m_forcedFacingDirection = Vector3.up;
		public Vector3 m_forcedFacingAxis = Vector3.up;
		public bool m_ignoreTwistLimits = false;
	}
}