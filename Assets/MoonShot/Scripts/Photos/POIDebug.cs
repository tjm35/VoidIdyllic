using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class POIDebug : MonoBehaviour
	{
		public PointOfInterest m_targetPOI;
		public PhotoEvaluator m_evaluator;
		public Camera m_testCamera;

		// Update is called once per frame
		void Update()
		{
			if (m_targetPOI && m_evaluator)
			{
				var context = new PhotoEvaluator.Context();
				context.m_camera = m_testCamera;
				Debug.Log($"{m_targetPOI.gameObject.name} visibility: {m_evaluator.GetVisibility(m_targetPOI, context)}");
			}
		}
	}
}