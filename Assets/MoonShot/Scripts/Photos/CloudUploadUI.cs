using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Moonshot.Photos
{
	public class CloudUploadUI : MonoBehaviour
	{
		public Button m_uploadButton;
		public TMP_Text m_uploadButtonText;
		public int m_photosNeeded = 2;
		public UnityEvent m_onUpload = new UnityEvent();

		public int MaxPhotos => m_photosNeeded;

		public bool IsPhotoSelected(PhotoSystem.IPhotoData i_photo)
		{
			return m_selectedPhotos.Contains(i_photo);
		}

		public bool SetPhotoSelected(PhotoSystem.IPhotoData i_photo, bool i_selected)
		{
			if (i_selected)
			{
				if (!m_selectedPhotos.Contains(i_photo) && m_selectedPhotos.Count < MaxPhotos)
				{
					m_selectedPhotos.Add(i_photo);
				}
			}
			else
			{
				if (m_selectedPhotos.Contains(i_photo))
				{
					m_selectedPhotos.Remove(i_photo);
				}
			}
			return IsPhotoSelected(i_photo);
		}

		public bool IsGoalMetBySelectedPhotos(Goal i_goal)
		{
			foreach (var photo in m_selectedPhotos)
			{
				if (photo.GoalsMet.Contains(i_goal))
				{
					return true;
				}
			}
			return false;
		}

		public void DoUpload()
		{
			m_onUpload.Invoke();
		}

		private void Update()
		{
			PruneDeletedPhotos();

			if (m_selectedPhotos.Count >= m_photosNeeded)
			{
				m_uploadButton.interactable = true;
				m_uploadButtonText.text = "Upload and Leave System!";
			}
			else
			{
				m_uploadButton.interactable = false;
				m_uploadButtonText.text = $"{m_selectedPhotos.Count}/{m_photosNeeded} photos selected";
			}
		}

		private void PruneDeletedPhotos()
		{
			var realPhotos = PhotoSystem.Instance.GetPhotos();
			m_selectedPhotos.RemoveAll(p => !realPhotos.Contains(p));

			while (m_selectedPhotos.Count > MaxPhotos)
			{
				m_selectedPhotos.RemoveRange(MaxPhotos, m_selectedPhotos.Count - MaxPhotos);
			}
		}

		private List<PhotoSystem.IPhotoData> m_selectedPhotos = new List<PhotoSystem.IPhotoData>();
	}
}