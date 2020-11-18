using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	[ExecuteAlways]
	public class RotatingFrame : MonoBehaviour
	{
		public Vector3 m_axis = Vector3.up;
		public float m_period = 10.0f;

		// Start is called before the first frame update
		void Start()
		{
			m_timeSource = transform.GetComponentInAncestors<OrreryTimeSource>() ?? OrreryTimeSource.Global;
		}

		// Update is called once per frame
		void Update()
		{
			float angle = m_timeSource.TimeElapsed * 360.0f / m_period;

			transform.localRotation = Quaternion.AngleAxis(angle, m_axis);
		}

		private OrreryTimeSource m_timeSource;
	}
}