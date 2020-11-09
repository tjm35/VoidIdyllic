using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.Photos
{
	public class GalleryUIEntry : MonoBehaviour
	{
		public RawImage m_image;
		public GameObject m_checkSpot;

		public PhotoSystem.IPhotoData Photo
		{
			get { return m_photo; }
			set { m_photo = value; m_image.texture = m_photo.PreviewTexture; UpdateGoals(); }
		}

		private void UpdateGoals()
		{
			foreach (var go in m_checkMarks)
			{
				Destroy(go);
			}
			m_checkMarks.Clear();

			if (m_checkSpot)
			{
				foreach (var goal in m_photo.GoalsMet)
				{
					var check = Instantiate(m_checkSpot, m_checkSpot.transform.parent);
					check.SetActive(true);
					m_checkMarks.Add(check);
				}
			}
		}

		private PhotoSystem.IPhotoData m_photo;
		private List<GameObject> m_checkMarks = new List<GameObject>();
	}
}