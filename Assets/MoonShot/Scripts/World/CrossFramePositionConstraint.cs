using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	[ExecuteAlways]
	public class CrossFramePositionConstraint : MonoBehaviour
	{
		public Transform m_target;
		public LocalFrame m_overrideOwnFrame;
		public float m_blendRate = 0.1f;

		void LateUpdate()
		{
			LocalFrame lf = (m_overrideOwnFrame != null ? m_overrideOwnFrame : LocalFrame.Get(transform));
			LocalFrame targetlf = LocalFrame.Get(m_target);

			Vector3 targetPos = LocalFrame.TransformPointToLocal(lf, LocalFrame.TransformPointToGlobal(targetlf, m_target.position));
			if (m_firstFrame)
			{
				transform.position = targetPos;
				m_firstFrame = false;
			}
			else
			{
				transform.position = Vector3.Lerp(transform.position, targetPos, m_blendRate);
			}
		}
		private bool m_firstFrame = true;
	}
}