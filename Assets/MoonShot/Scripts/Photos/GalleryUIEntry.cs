using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.Photos
{
	public class GalleryUIEntry : MonoBehaviour
	{
		public RawImage m_image;

		public PhotoSystem.IPhotoData Photo
		{
			get { return m_photo; }
			set { m_photo = value; m_image.texture = m_photo.PreviewTexture; }
		}

		private PhotoSystem.IPhotoData m_photo;
	}
}