using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering.Universal;
using System.IO;
using System.Linq;
using System.Threading;
using System;
using Unity.Collections;

namespace Moonshot.Photos
{
	public class PhotoSystem : MonoBehaviour
	{
		public static PhotoSystem Instance { get; private set; }
		public PhotoEvaluator m_evaluator;
		public Material m_linearToGammaMat;
		public bool m_usePrefillPhotos;
		public List<PrefillPhoto> m_prefillPhotos;
		public GoalNotifications m_notifications;

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

			Tutorial.TutorialHelper.SetBool("HasTakenPhoto");
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

			Profiler.BeginSample("RecordCamera");
			RenderTexture gpuTexture = RecordCameraToNewGPUTexture(i_camera);
			Profiler.EndSample();
			i_camera.enabled = i_wasEnabled;

			while (!i_context.m_ready)
			{
				yield return new WaitForEndOfFrame();
			}

			m_evaluator.NotifyPhotoTaken(i_context);
			var completedGoals = m_evaluator.EvaluateGoals(i_context);
			foreach (var goal in completedGoals)
			{
				m_notifications.NotifyGoal(goal, m_completedGoals.Contains(goal) == false);
				m_completedGoals.Add(goal);
			}

			var data = new PhotoData();
			data.PreviewTexture = gpuTexture;
			data.FullTexture = gpuTexture;
			data.GoalsMet = completedGoals.ToArray();
			m_photos.Add(data);

			if (m_photos.Count >= 4)
			{
				Tutorial.TutorialHelper.SetBool("IsHasMultiplePhotos");
			}

			// Some very aggressive delays to try to minimise the chance of stalling waiting for the GPU.
			{
				Profiler.BeginSample("GammaCorrect");
				RenderTexture gammaTexture = GammaCorrectToNewGPUTexture(gpuTexture);
				Profiler.EndSample();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				Profiler.BeginSample("RecordTexture");
				Texture2D readableSaveTexture = RecordTextureToNewReadableTexture(gammaTexture);
				Profiler.EndSample();
				gammaTexture.Release();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				yield return new WaitForEndOfFrame();
				Profiler.BeginSample("SavePicture");
				SavePicture(readableSaveTexture);
				Profiler.EndSample();
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

		private RenderTexture GammaCorrectToNewGPUTexture(Texture i_source)
		{
			int width = i_source.width;
			int height = i_source.height;
			RenderTexture gammaTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBFloat);

			Graphics.Blit(i_source, gammaTexture, m_linearToGammaMat);

			return gammaTexture;
		}

		private Texture2D RecordTextureToNewReadableTexture(RenderTexture i_source)
		{
			int width = i_source.width;
			int height = i_source.height;
			Texture2D readableSaveTexture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);

			Graphics.SetRenderTarget(i_source);
			readableSaveTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			Graphics.SetRenderTarget(null);
			readableSaveTexture.Apply();

			return readableSaveTexture;
		}

		private void SavePicture(Texture2D i_readableTexture)
		{
			string screenshotsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "VoidIdyllic");
			if (!Directory.Exists(screenshotsPath))
			{
				Directory.CreateDirectory(screenshotsPath);
			}
			DateTime now = DateTime.Now;
			string filename = Application.productName + " " + now.ToString("yyyyMMdd HHmmss");
			string savePath = Path.Combine(screenshotsPath, filename);
			if (File.Exists(savePath + ".png"))
			{
				int i = 1;
				while (File.Exists(savePath + " " + i.ToString() + ".png") && i < 1000)
				{
					i++;
				}
				savePath += " " + i.ToString();
			}
			savePath += ".png";
			Color32[] colorData = i_readableTexture.GetPixels32(0);
			int width = i_readableTexture.width;
			int height = i_readableTexture.height;

			// Perform PNG encoding on a fire-and-forget thread to avoid stalling the game.
			// This operates purely on copies of data and we never need to access the result so no synchronisation is needed.
			new Thread
			(
				() =>
				{
					var builder = BigGustave.PngBuilder.Create(width, height, false);
					for (int y = 0; y < height; ++y)
					{
						for (int x = 0; x < width; ++x)
						{
							Color32 col = colorData[y * width + x];
							builder.SetPixel(col.r, col.g, col.b, x, height - y - 1);
						}
					}

					using (var file = new FileStream(savePath, FileMode.CreateNew))
					{
						builder.Save(file);
					}
				}
			).Start();
		}

		private HashSet<Goal> m_completedGoals = new HashSet<Goal>();
		private List<PhotoData> m_photos = new List<PhotoData>();
	}
}