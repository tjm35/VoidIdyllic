using System.Collections;
using System.Collections.Generic;
using Moonshot.World;
using UnityEngine;

namespace Moonshot.Photos
{
	public class PhotoEvaluator : MonoBehaviour
	{
		public struct Context
		{
			public Camera m_camera;
		}

		public Context ConstructContext(Camera i_camera)
		{
			var context = new Context();
			context.m_camera = i_camera;
			return context;
		}

		public float GetVisibility(PointOfInterest i_poi, Context i_context)
		{
			return 0.5f;
		}
	}
}