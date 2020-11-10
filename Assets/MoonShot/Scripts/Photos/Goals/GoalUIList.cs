using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class GoalUIList : MonoBehaviour
	{
		public Transform GoalsList;
		public GameObject GoalUIPrefab;

		private void OnEnable()
		{
			FullPhotoUI fpu = transform.GetComponentInAncestors<FullPhotoUI>();

			IEnumerable<Goal> goals = null;
			if (GoalsList)
			{
				goals = GoalsList.GetComponentsInChildren<Goal>();
			}

			if (fpu)
			{
				goals = fpu.Photo.GoalsMet;
			}

			if (goals != null)
			{ 
				foreach (var goal in goals)
				{
					var gu = Instantiate(GoalUIPrefab, transform);
					var gue = gu.GetComponent<GoalUIEntry>();
					if (gue)
					{
						gue.Goal = goal;
					}
					m_uiEntries.Add(gu);
				}
			}
		}

		private void OnDisable()
		{
			foreach (var go in m_uiEntries)
			{
				Destroy(go);
			}
			m_uiEntries.Clear();
		}

		private List<GameObject> m_uiEntries = new List<GameObject>();
	}
}