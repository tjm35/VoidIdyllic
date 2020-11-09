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
			set { m_photo = value; m_image.texture = m_photo.PreviewTexture; UpdateGoals(); UpdateToggle(); }
		}

		private void Start()
		{
			m_cloudUI = transform.GetComponentInAncestors<CloudUploadUI>();
			m_toggle = GetComponent<Toggle>();
			if (m_toggle)
			{
				m_toggle.onValueChanged.AddListener(OnToggleChanged);
			}
		}

		private void OnToggleChanged(bool i_value)
		{
			if (m_cloudUI && m_photo != null)
			{
				bool acceptedValue = m_cloudUI.SetPhotoSelected(m_photo, i_value);
				if (acceptedValue != i_value && m_toggle)
				{
					m_toggle.isOn = acceptedValue;
				}
			}
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

		private void UpdateToggle()
		{
			if (m_toggle && m_cloudUI && m_photo != null)
			{
				m_toggle.isOn = m_cloudUI.IsPhotoSelected(m_photo);
			}
		}

		private PhotoSystem.IPhotoData m_photo;
		private List<GameObject> m_checkMarks = new List<GameObject>();
		private CloudUploadUI m_cloudUI;
		private Toggle m_toggle;
	}
}