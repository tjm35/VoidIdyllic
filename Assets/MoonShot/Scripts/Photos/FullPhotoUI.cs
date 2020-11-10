using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class FullPhotoUI : MonoBehaviour
	{
		public Material PreviewMaterial;
		public RenderTexture ViewfinderTexture;
		public PhotoSystem.IPhotoData Photo { get; set; }

		private void OnEnable()
		{
			PreviewMaterial.SetTexture("_EmissionMap", Photo.FullTexture);
		}

		private void OnDisable()
		{
			PreviewMaterial.SetTexture("_EmissionMap", ViewfinderTexture);
		}
	}
}