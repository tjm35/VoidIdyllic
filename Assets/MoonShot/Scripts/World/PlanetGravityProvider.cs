using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	public class PlanetGravityProvider : MonoBehaviour, IGravityProvider
	{
		public float m_radius;
		public float m_surfaceGravityGs;

		void Start()
		{
			if (GlobalGravityProvider.Instance)
			{
				GlobalGravityProvider.Instance.PlanetProviders.Add(this);
			}
		}

		void OnDestroy()
		{
			if (GlobalGravityProvider.Instance)
			{
				GlobalGravityProvider.Instance.PlanetProviders.Remove(this);
			}
		}

		public bool GetUp(Vector3 i_point, out Vector3 o_up)
		{
			Vector3 gravity = GetGravity(i_point);
			o_up = -gravity.normalized;
			return gravity.sqrMagnitude > 0.001f;
		}

		public Vector3 GetGravity(Vector3 i_point)
		{
			Vector3 localPoint = transform.InverseTransformPoint(i_point);
			if (localPoint.sqrMagnitude > m_radius * m_radius)
			{
				float distance = localPoint.magnitude;
				float distanceRatio = distance / m_radius;
				float inverseSquare = (1.0f / (distanceRatio * distanceRatio));

				return -inverseSquare * m_surfaceGravityGs * transform.TransformDirection(localPoint.normalized);
			}
			else if (localPoint.sqrMagnitude > 0.1f)
			{
				float distance = localPoint.magnitude;
				float distanceRatio = distance / m_radius;

				return -distanceRatio * m_surfaceGravityGs * transform.TransformDirection(localPoint.normalized);
			}
			else
			{
				return Vector3.zero;
			}
		}

		public float GetGravityProminence(Vector3 i_point)
		{
			Vector3 localPoint = transform.InverseTransformPoint(i_point);
			float distance = Mathf.Max(localPoint.magnitude, m_radius);
			float distanceRatio = distance / m_radius;
			float inverseSquare = (1.0f / (distanceRatio * distanceRatio));

			return inverseSquare * m_surfaceGravityGs;
		}

		public IGravityProvider GetMostProminent(Vector3 i_point)
		{
			return this;
		}
	}
}