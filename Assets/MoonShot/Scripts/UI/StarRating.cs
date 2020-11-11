using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Moonshot.UI
{
	public class StarRating : MonoBehaviour
	{
		public GameObject m_starProto;
		public UnityEvent OnStarComplete = new UnityEvent();
		[SerializeField]
		private float m_rating = 0.0f;

		public float Rating
		{
			get { return m_rating; }
			set { m_rating = value; UpdateRating(); }
		}

		public void ChangeDisplayProp(float i_prop)
		{
			int lastStarsComplete = Mathf.FloorToInt(m_displayProp * m_rating);
			m_displayProp = i_prop;
			int newStarsComplete = Mathf.FloorToInt(m_displayProp * m_rating);
			UpdateRating();

			if (newStarsComplete > lastStarsComplete)
			{
				var go = m_stars[newStarsComplete - 1];
				if (go.GetComponent<Animation>())
				{
					go.GetComponent<Animation>().Play("RatingStarPulse");
				}
				OnStarComplete.Invoke();
			}
		}

		private void Start()
		{
			UpdateRating();
		}

		private void UpdateRating()
		{
			float displayRating = m_rating * m_displayProp;
			int partialStars = Mathf.CeilToInt(displayRating);
			int fullStars = Mathf.FloorToInt(displayRating);

			while (m_stars.Count > partialStars)
			{
				var go = m_stars[m_stars.Count - 1];
				m_stars.RemoveAt(m_stars.Count - 1);
				Destroy(go);
			}
			for (int i = 0; i < partialStars; i++)
			{
				if (m_stars.Count <= i)
				{
					var go = Instantiate(m_starProto, this.transform);
					go.SetActive(true);
					m_stars.Add(go);
				}
				if (i < fullStars)
				{
					SetPartial(m_stars[i], 1.0f);
				}
				else
				{
					SetPartial(m_stars[i], displayRating - (float)fullStars);
				}
			}
		}

		private void SetPartial(GameObject i_star, float i_partial)
		{
			var rt = i_star.transform.GetChild(0).GetComponent<RectTransform>();
			rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, rt.rect.height * i_partial);
		}

		private List<GameObject> m_stars = new List<GameObject>();
		private float m_displayProp = 0.0f;
	}
}