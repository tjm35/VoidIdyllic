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
		public bool m_usePrefillPhotos;
		public List<PrefillPhoto> m_prefillPhotos;

		[Serializable]
		public struct PrefillPhoto
		{
			public Texture2D m_image;
			public List<Goal> m_goals;
		}

		public interface IPhotoData
		{
			Texture PreviewTexture { get; }
			Texture FullTexture { get; }
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
			Debug.Assert(i_data is PhotoData);
			if (!((PhotoData)i_data).IsAsset)
			{
				if (i_data.PreviewTexture != i_data.FullTexture)
				{
					Destroy(i_data.PreviewTexture);
				}
				Destroy(i_data.FullTexture);
			}
			m_photos.Remove((PhotoData)i_data);
		}

		private IEnumerator TakePhotoCoroutine(Camera i_camera, bool i_wasEnabled, PhotoEvaluator.Context i_context)
		{
			yield return new WaitForEndOfFrame();

			RenderTexture gpuTexture = RecordCameraToNewGPUTexture(i_camera);
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
			data.PreviewTexture = gpuTexture;
			data.FullTexture = gpuTexture;
			data.GoalsMet = completedGoals.ToArray();
			m_photos.Add(data);

			{
				Texture2D readableSaveTexture = GammaCorrectAndRecordTextureToNewReadableTexture(gpuTexture);
				EditorSavePicture(readableSaveTexture);
			}
		}

		private class PhotoData : IPhotoData
		{
			public Texture PreviewTexture { get; set; }
			public Texture FullTexture { get; set; }
			public IEnumerable<Goal> GoalsMet { get; set; }
			public bool IsAsset = false;
		}

		private void Awake()
		{
			Debug.Assert(Instance == null);
			Instance = this;
		}

		private void Start()
		{
			if (m_usePrefillPhotos)
			{
				foreach (var pf in m_prefillPhotos)
				{
					PhotoData pd = new PhotoData();
					pd.FullTexture = pf.m_image;
					pd.PreviewTexture = pf.m_image;
					pd.GoalsMet = pf.m_goals ?? new List<Goal>();
					pd.IsAsset = true;
					m_photos.Add(pd);
				}
			}
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

		private RenderTexture RecordCameraToNewGPUTexture(Camera i_camera)
		{
			int width = i_camera.targetTexture.width;
			int height = i_camera.targetTexture.height;
			RenderTexture gpuTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);

			Graphics.Blit(i_camera.targetTexture, gpuTexture);

			return gpuTexture;
		}

		private Texture2D GammaCorrectAndRecordTextureToNewReadableTexture(Texture i_source)
		{
			int width = i_source.width;
			int height = i_source.height;
			RenderTexture gammaTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);

			Graphics.Blit(i_source, gammaTexture, m_linearToGammaMat);

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