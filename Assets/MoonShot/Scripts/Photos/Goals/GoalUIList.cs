using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class GoalUIList : MonoBehaviour
	{
		public Transform GoalsList;
		public GameObject GoalUIPrefab;

		private void Start()
		{
			if (GoalsList)
			{
				foreach (var goal in GoalsList.GetComponentsInChildren<Goal>())
				{
					var gu = Instantiate(GoalUIPrefab, transform);
					var gue = gu.GetComponent<GoalUIEntry>();
					if (gue)
					{
						gue.Goal = goal;
					}
				}
			}
		}
	}
}