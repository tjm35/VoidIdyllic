using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class Goal : MonoBehaviour
	{
		public interface IRequirement
		{
			bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context);
		}

		public string m_description;

		public bool Check(PhotoEvaluator i_evaluator, PhotoEvaluator.Context i_context)
		{
			Debug.Assert(i_evaluator, "Goal.Check: Must provide evaluator.");
			Debug.Assert(i_context != null, "Goal.Check: Must provide context.");
			Debug.Assert(i_context.m_ready, "Goal.Check: Context must be ready for analysis.");

			for (int i = 0; i < m_requirements.Length; ++i)
			{
				var req = m_requirements[i];
				Debug.Assert(req != null, "Goal.Check: Missing requirement. Has it been destroyed?");
				if (m_requirements[i].Check(i_evaluator, i_context) == false)
				{
					// Failing any requirement fails the goal.
					return false;
				}
			}

			// All requirements met!
			return true;
		}

		private void Start()
		{
			m_requirements = GetComponents<IRequirement>();

			Debug.Assert(m_requirements != null && m_requirements.Length > 0, "Goal: Must have at least one requirement.");
		}

		private IRequirement[] m_requirements;
	}
}