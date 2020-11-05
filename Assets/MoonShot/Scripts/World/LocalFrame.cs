using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	public class LocalFrame : MonoBehaviour
	{
		public Transform GlobalLocation;

		public Vector3 TransformPointToGlobal(Vector3 i_localPoint)
		{
			return GlobalLocation.TransformPoint(transform.InverseTransformPoint(i_localPoint));
		}

		public Vector3 TransformVectorToGlobal(Vector3 i_localVector)
		{
			return GlobalLocation.TransformVector(transform.InverseTransformVector(i_localVector));
		}

		public Quaternion TransformRotationToGlobal(Quaternion i_localRotation)
		{
			return GlobalLocation.rotation * (Quaternion.Inverse(transform.rotation) * i_localRotation);
		}

		public Vector3 TransformPointToLocal(Vector3 i_globalPoint)
		{
			return transform.TransformPoint(GlobalLocation.InverseTransformPoint(i_globalPoint));
		}

		public Vector3 TransformVectorToLocal(Vector3 i_globalVector)
		{
			return transform.TransformVector(GlobalLocation.InverseTransformVector(i_globalVector));
		}

		public Quaternion TransformRotationToLocal(Quaternion i_globalRotation)
		{
			return transform.rotation * (Quaternion.Inverse(GlobalLocation.rotation) * i_globalRotation);
		}

		public static LocalFrame Get(Transform i_object)
		{
			Transform t = i_object;
			while (t != null && t.GetComponent<LocalFrame>() == null)
			{
				t = t.parent;
			}
			return t != null ? t.GetComponent<LocalFrame>() : null;
		}

		public static Vector3 TransformPointToGlobal(LocalFrame i_frame, Vector3 i_localPoint)
		{
			return i_frame ? i_frame.TransformPointToGlobal(i_localPoint) : i_localPoint;
		}

		public static Vector3 TransformPointToLocal(LocalFrame i_frame, Vector3 i_globalPoint)
		{
			return i_frame ? i_frame.TransformPointToLocal(i_globalPoint) : i_globalPoint;
		}

		public static Vector3 TransformVectorToLocal(LocalFrame i_frame, Vector3 i_globalVector)
		{
			return i_frame ? i_frame.TransformVectorToLocal(i_globalVector) : i_globalVector;
		}
	}
}