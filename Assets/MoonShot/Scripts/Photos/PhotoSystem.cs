using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Linq;
using System;

namespace Moonshot.Photos
{
	public class PhotoSystem : MonoBehaviour
	{
		public static PhotoSystem Instance { get; private set; }
		public PhotoEvaluator m_evaluator;
		public Material m_linearToGammaMat;

		public interface IPhotoData
		{
			Texture2D PreviewTexture { get; }
			Texture2D FullTexture { get; }
			IEnumerable<Goal> GoalsMet { get; }
		}

		public void TakePhoto(Camera i_camera)
		{
			bool wasEnabled = i_camera.enabled;
			i_camera.enabled = true;
			var context = m_evaluator.ConstructContext(i_camera);
			StartCoroutine(TakePhotoCoroutine(i_camera, wasEnabled, context));
		}

		public bool IsGoalComplete(Goal i_goal)
		{
			return m_completedGoals.Contains(i_goal);
		}

		public IEnumerable<IPhotoData> GetPhotos()
		{
			return m_photos.Cast<IPhotoData>();
		}

		public void DeletePhoto(IPhotoData i_data)
		{
			if (i_data.PreviewTexture != i_data.FullTexture)
			{
				Destroy(i_data.PreviewTexture);
			}
			Destroy(i_data.FullTexture);
			m_photos.Remove((PhotoData)i_data);
		}

		private IEnumerator TakePhotoCoroutine(Camera i_camera, bool i_wasEnabled, PhotoEvaluator.Context i_context)
		{
			yield return new WaitForEndOfFrame();

			Texture2D readableSaveTexture = RecordCameraToNewReadableTexture(i_camera);
			i_camera.enabled = i_wasEnabled;

			while (!i_context.m_ready)
			{
				yield return new WaitForEndOfFrame();
			}

			var completedGoals = m_evaluator.EvaluateGoals(i_context);
			foreach (var goal in completedGoals)
			{
				m_completedGoals.Add(goal);
			}

			var data = new PhotoData();
			data.PreviewTexture = readableSaveTexture;
			data.FullTexture = readableSaveTexture;
			data.GoalsMet = completedGoals.ToArray();
			m_photos.Add(data);

			EditorSavePicture(readableSaveTexture);
		}

		private class PhotoData : IPhotoData
		{
			public Texture2D PreviewTexture { get; set; }
			public Texture2D FullTexture { get; set; }
			public IEnumerable<Goal> GoalsMet { get; set; }
		}

		private void Awake()
		{
			Debug.Assert(Instance == null);
			Instance = this;
		}

		private void OnDestroy()
		{
			while (m_photos.Any())
			{
				DeletePhoto(m_photos.First());
			}

			Debug.Assert(Instance == this);
			Instance = null;
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

		private HashSet<Goal> m_completedGoals = new HashSet<Goal>();
		private List<PhotoData> m_photos = new List<PhotoData>();
	}
}