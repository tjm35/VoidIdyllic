using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class GalleryUIList : MonoBehaviour
	{
		public GameObject m_photoEntryPrefab;
		public GameObject m_noPhotosMessage;

		public void RefreshList()
		{
			ClearOldList();

			var photos = PhotoSystem.Instance.GetPhotos();

			foreach (var photo in photos)
			{
				var entry = Instantiate(m_photoEntryPrefab, transform);
				var gue = entry.GetComponent<GalleryUIEntry>();
				if (gue)
				{
					gue.Photo = photo;
				}
				m_entries.Add(entry);
			}

			m_noPhotosMessage.SetActive(m_entries.Count == 0);
		}

		private void OnEnable()
		{
			RefreshList();
		}

		private void ClearOldList()
		{
			foreach (var go in m_entries)
			{
				Destroy(go);
			}
			m_entries.Clear();
		}

		private List<GameObject> m_entries = new List<GameObject>();
	}
}