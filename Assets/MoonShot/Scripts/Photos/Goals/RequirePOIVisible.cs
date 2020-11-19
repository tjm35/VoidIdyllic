using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using UnityEngine;

namespace Moonshot.Photos.Requirements
{
	public class RequirePOIVisible : MonoBehaviour, Goal.IRequirement
	{
		public PointOfInterest m_pointOfInterest;
		public int m_minVisibility = 1;
		public bool m_matchByName = false;
		public string m_matchName = "";

		public bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context)
		{
			if (m_matchByName)
			{
				return i_evaluator.GetVisiblePOIs(m_minVisibility, i_context).Any(poi => poi.gameObject.name.Equals(m_matchName, StringComparison.InvariantCultureIgnoreCase));
			}
			else
			{
				int visibility = i_evaluator.GetVisibility(m_pointOfInterest, i_context);
				return visibility >= m_minVisibility;
			}
		}
	}
}