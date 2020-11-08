using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moonshot.Photos.Requirements
{
	public class RequirePOICount : MonoBehaviour, Goal.IRequirement
	{
		public PointOfInterest.Class m_pointOfInterestClass;
		public int m_minVisibility = 1;
		public int m_minCount = 1;

		public bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context)
		{
			int count = i_evaluator.GetVisiblePOIs(m_minVisibility, i_context).Where(poi => (poi.m_class & m_pointOfInterestClass) == m_pointOfInterestClass).Count();
			//Debug.Log($"RequirePOICount: {count} visible.");
			return count >= m_minCount;
		}
	}
}