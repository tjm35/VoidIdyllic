using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos.Requirements
{
	public class RequirePOIVisible : MonoBehaviour, Goal.IRequirement
	{
		public PointOfInterest m_pointOfInterest;
		public int m_minVisibility = 1;

		public bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context)
		{
			int visibility = i_evaluator.GetVisibility(m_pointOfInterest, i_context);
			return visibility >= m_minVisibility;
		}
	}
}