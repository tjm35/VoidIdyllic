﻿using Moonshot.Planet;
using Moonshot.Player;
using System.Linq;
using UnityEngine;

namespace Moonshot.Photos.Requirements
{
	public class RequirePartialEclipse : MonoBehaviour, Goal.IRequirement
	{
		public OrreryPlanet m_eclipsee;
		public int m_minEclipseeVisibility = 1;
		public int m_minEclipsorVisibility = 1;
		[Tooltip("The eclipsor's visibility must be at least this multiple of the eclipsee's (as well as its usual limit).")]
		public float m_minVisibilityRatio = 0.0f;

		public bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context)
		{
			// Eclipsee must be visibile.
			int eclipseeVisibility = i_evaluator.GetVisibility(m_eclipsee.m_pointOfInterest, i_context);
			if (eclipseeVisibility < m_minEclipseeVisibility)
			{
				return false;
			}
			Vector3 eclipseePos = i_evaluator.GetGlobalPOIPosition(m_eclipsee.m_pointOfInterest, i_context);
			Vector3 cameraPos = i_context.m_globalCameraPos;
			Vector3 cameraToEclipsee = eclipseePos - cameraPos;
			float cameraToEclipseeDistance = cameraToEclipsee.magnitude;
			Vector3 cameraToEclipseeDirection = cameraToEclipsee.normalized;

			int minEclipsorVisibility = Mathf.Max(m_minEclipsorVisibility, (int)(m_minVisibilityRatio * (float)(eclipseeVisibility)));

			var eclipsors = i_evaluator.GetVisiblePOIs(minEclipsorVisibility, i_context).Where(poi => poi.GetComponent<OrreryPlanet>() != null && poi.gameObject != m_eclipsee.gameObject);
			foreach (PointOfInterest eclipsor in eclipsors)
			{
				if (eclipsor.transform == i_context.m_globalLocation && PlayerVehicle.Current.m_type == PlayerVehicle.VehicleType.OnFoot)
				{
					// Disqualify the eclipsor you're currently standing on.
					continue;
				}

				Vector3 eclipsorPos = i_evaluator.GetGlobalPOIPosition(eclipsor, i_context);

				Vector3 cameraToEclipsor = eclipsorPos - cameraPos;
				float cameraToEclipsorDistanceAlongEclipseeLine = Vector3.Dot(cameraToEclipsor, cameraToEclipseeDirection);
				if (cameraToEclipsorDistanceAlongEclipseeLine > 0.0f && cameraToEclipsorDistanceAlongEclipseeLine < cameraToEclipseeDistance)
				{
					float eclipsorDistanceProp = cameraToEclipsorDistanceAlongEclipseeLine / cameraToEclipseeDistance;
					float effectiveEclipseeRadius = eclipsorDistanceProp * m_eclipsee.Radius;
					float eclipsorRadius = eclipsor.GetComponent<OrreryPlanet>().Radius;
					float offAxisDistance = (cameraToEclipsor - cameraToEclipseeDirection * cameraToEclipsorDistanceAlongEclipseeLine).magnitude;

					if (offAxisDistance < eclipsorRadius + effectiveEclipseeRadius)
					{
						//Debug.Log($"{m_eclipsee.gameObject.name} eclipsed by {eclipsor.gameObject.name}: offaxisDistance ({offAxisDistance}) < eclipsorRadius ({eclipsorRadius}) + effectiveEclipseeRadius ({effectiveEclipseeRadius}) at distance {cameraToEclipsorDistanceAlongEclipseeLine}");
						return true;
					}
				}
			}

			return false;
		}
	}
}