using System.Collections;
using System.Collections.Generic;
using Moonshot.UI;
using UnityEngine;

namespace Moonshot.Photos
{
	[RequireComponent(typeof(NotificationQueue))]
	public class GoalNotifications : MonoBehaviour
	{
		public GameObject GoalUIPrefab;

		public void NotifyGoal(Goal i_goal, bool i_firstTime)
		{
			if (!m_currentDisplayed.Contains(i_goal))
			{
				var gu = Instantiate(GoalUIPrefab, transform);
				var gue = gu.GetComponent<GoalUIEntry>();
				if (gue)
				{
					gue.Goal = i_goal;
				}
				m_currentDisplayed.Add(i_goal);
				m_queue.RegisterNotification(gu);
			}
			m_queue.PokeNotification();
		}

		private void Start()
		{
			m_queue = GetComponent<NotificationQueue>();
		}

		private void OnNotificationsCleared()
		{
			m_currentDisplayed.Clear();
		}

		private NotificationQueue m_queue;
		private HashSet<Goal> m_currentDisplayed = new HashSet<Goal>();
	}
}