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
				if (m_context == null)
				{
					m_context = m_evaluator.ConstructContext(m_testCamera);
				}
				if (m_context.m_ready)
				{
					Debug.Log($"{m_targetPOI.gameObject.name} visibility: {m_evaluator.GetVisibility(m_targetPOI, m_context)}");
					m_context = null;
				}
			}
		}

		private PhotoEvaluator.Context m_context;
	}
}