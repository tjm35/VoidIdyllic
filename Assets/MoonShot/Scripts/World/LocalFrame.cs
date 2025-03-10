﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	public class LocalFrame : MonoBehaviour
	{
		public Transform GlobalLocation;
		public bool GlobalLocationIsTemporary = false;

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

		public static Vector3 TransformVectorToGlobal(LocalFrame i_frame, Vector3 i_localVector)
		{
			return i_frame ? i_frame.TransformVectorToGlobal(i_localVector) : i_localVector;
		}

		public static Vector3 TransformVectorToLocal(LocalFrame i_frame, Vector3 i_globalVector)
		{
			return i_frame ? i_frame.TransformVectorToLocal(i_globalVector) : i_globalVector;
		}

		public static Quaternion TransformRotationToGlobal(LocalFrame i_frame, Quaternion i_localRotation)
		{
			return i_frame ? i_frame.TransformRotationToGlobal(i_localRotation) : i_localRotation;
		}

		public static Quaternion TransformRotationToLocal(LocalFrame i_frame, Quaternion i_globalRotation)
		{
			return i_frame ? i_frame.TransformRotationToLocal(i_globalRotation) : i_globalRotation;
		}

		public static Vector3 GetGlobalPosition(Transform i_transform)
		{
			return TransformPointToGlobal(Get(i_transform), i_transform.position);
		}

		public static void Transplant(Transform i_transform, LocalFrame i_oldFrame, LocalFrame i_newFrame)
		{
			Vector3 pos = TransformPointToGlobal(i_oldFrame, i_transform.position);
			Quaternion rot = TransformRotationToGlobal(i_oldFrame, i_transform.rotation);
			i_transform.position = TransformPointToLocal(i_newFrame, pos);
			i_transform.rotation = TransformRotationToLocal(i_newFrame, rot);

		}
	}
}