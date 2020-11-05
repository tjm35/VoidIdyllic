using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	[ExecuteAlways]
	public class PositionInGlobalFrame : MonoBehaviour
	{
		void LateUpdate()
		{
			LocalFrame lf = LocalFrame.Get(transform);

			if (lf)
			{
				Vector3 localFramePosition = transform.parent.position;
				transform.position = lf.TransformPointToGlobal(localFramePosition);

				Quaternion localFrameRotation = transform.parent.rotation;
				transform.rotation = lf.TransformRotationToGlobal(localFrameRotation);
			}
			else
			{
				transform.localPosition = Vector3.zero;
				transform.localRotation = Quaternion.identity;
			}
		}
	}
}