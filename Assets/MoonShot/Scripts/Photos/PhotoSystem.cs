using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.IO;
using System;

namespace Moonshot.Photos
{
	public class PhotoSystem : MonoBehaviour
	{
		public Material m_linearToGammaMat;

		public void TakePhoto(Camera i_camera)
		{
			i_camera.enabled = true;
			StartCoroutine(TakePhotoCoroutine(i_camera));
		}

		public IEnumerator TakePhotoCoroutine(Camera i_camera)
		{
			yield return new WaitForEndOfFrame();

			Texture2D readableSaveTexture = RecordCameraToNewReadableTexture(i_camera);

			EditorSavePicture(readableSaveTexture);

			Destroy(readableSaveTexture);
			i_camera.enabled = false;
		}

		private Texture2D RecordCameraToNewReadableTexture(Camera i_camera)
		{
			int width = i_camera.targetTexture.width;
			int height = i_camera.targetTexture.height;
			RenderTexture gammaTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);

			Graphics.Blit(i_camera.targetTexture, gammaTexture, m_linearToGammaMat);

			Texture2D readableSaveTexture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);

			Graphics.SetRenderTarget(gammaTexture);
			readableSaveTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			Graphics.SetRenderTarget(null);
			readableSaveTexture.Apply();
			gammaTexture.Release();

			return readableSaveTexture;
		}

		private void EditorSavePicture(Texture2D i_readableTexture)
		{
			if (Application.isEditor)
			{
				string screenshotsPath = Path.Combine(Application.dataPath, "..", "Screenshots");
				if (!Directory.Exists(screenshotsPath))
				{
					Directory.CreateDirectory(screenshotsPath);
				}
				DateTime now = DateTime.Now;
				string filename = Application.productName + " " + now.ToString("yyyyMMdd HHmmss") + ".png";
				string savePath = Path.Combine(screenshotsPath, filename);

				byte[] pngData = i_readableTexture.EncodeToPNG();
				File.WriteAllBytes(savePath, pngData);
			}
		}
	}
}