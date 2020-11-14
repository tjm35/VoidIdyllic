using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.UI
{
	public class NotificationQueue : MonoBehaviour
	{
		public float m_stayOnTime = 3.0f;
		public float m_scrollRate = 150.0f;

		public void RegisterNotification(GameObject i_object)
		{
			m_notifications.Add(i_object);
			PokeNotification();
		}

		public void PokeNotification()
		{
			m_fadeoutTimer = m_stayOnTime;
		}

		public float CurrentHeight
		{
			get
			{
				return m_rectTransform.anchoredPosition.y;
			}
			set
			{
				m_rectTransform.anchoredPosition = new Vector2(m_rectTransform.anchoredPosition.x, value);
			}
		}

		public float RequiredHeight => m_rectTransform.rect.height;

		private void Start()
		{
			m_rectTransform = GetComponent<RectTransform>();
		}

		// Update is called once per frame
		private void Update()
		{
			if (m_notifications.Count > 0)
			{ 
				m_fadeoutTimer -= Time.unscaledDeltaTime;
				if (m_fadeoutTimer > 0.0f)
				{
					if (CurrentHeight < RequiredHeight)
					{
						CurrentHeight = Mathf.Min(CurrentHeight + Time.deltaTime * m_scrollRate, RequiredHeight);
					}
				}
				else
				{
					if (CurrentHeight > 0.0f)
					{
						CurrentHeight -= Time.deltaTime * m_scrollRate;
					}
					if (CurrentHeight <= 0.0f)
					{
						ClearNotifications();
						CurrentHeight = 0.0f;
					}
				}
			}
		}

		private void ClearNotifications()
		{
			foreach (var go in m_notifications)
			{
				Destroy(go);
			}
			m_notifications.Clear();
			SendMessage("OnNotificationsCleared", SendMessageOptions.DontRequireReceiver);
		}

		private float m_fadeoutTimer = 0.0f;
		private List<GameObject> m_notifications = new List<GameObject>();
		private RectTransform m_rectTransform;
	}
}