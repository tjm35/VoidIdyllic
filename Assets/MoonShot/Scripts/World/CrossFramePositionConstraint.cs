using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	[ExecuteAlways]
	public class CrossFramePositionConstraint : MonoBehaviour
	{
		public Transform m_target;

		void LateUpdate()
		{
			LocalFrame lf = LocalFrame.Get(transform);
			LocalFrame targetlf = LocalFrame.Get(m_target);

			transform.position = LocalFrame.TransformPointToLocal(lf, LocalFrame.TransformPointToGlobal(targetlf, m_target.position));
		}
	}
}