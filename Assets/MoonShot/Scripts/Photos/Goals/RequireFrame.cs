using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos.Requirements
{
	public class RequireFrame : MonoBehaviour, Goal.IRequirement
	{
		public Transform RequiredLocation;

		public bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context)
		{
			return i_context.m_globalLocation == RequiredLocation;
		}
	}
}