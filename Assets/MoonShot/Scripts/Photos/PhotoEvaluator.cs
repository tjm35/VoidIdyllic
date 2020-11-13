using System;
using System.Collections;
using System.Collections.Generic;
using Moonshot.World;
using UnityEngine;
using System.Linq;

namespace Moonshot.Photos
{
	public class PhotoEvaluator : MonoBehaviour
	{
		public Transform m_globalGoals;

		public class Context
		{
			public Camera m_analysisCamera;
			public bool m_ready;
			public Transform m_globalLocation;
			public Dictionary<PointOfInterest, int> m_visibility;
		}

		public Context ConstructContext(Camera i_viewCamera)
		{
			var context = new Context();
			context.m_analysisCamera = i_viewCamera.GetComponent<POIAnalysisCameraRef>().m_poiAnalysisCamera;
			var lf = LocalFrame.Get(i_viewCamera.transform);
			if (lf)
			{
				context.m_globalLocation = lf.GlobalLocation;
			}
			context.m_ready = false;
			StartCoroutine(PrepareContextCoroutine(context));
			return context;
		}

		public int GetVisibility(PointOfInterest i_poi, Context i_context)
		{
			if (i_context.m_visibility.ContainsKey(i_poi))
			{
				return i_context.m_visibility[i_poi];
			}
			else
			{
				return 0;
			}
		}

		public IEnumerable<PointOfInterest> GetVisiblePOIs(int i_minVisibility, Context i_context)
		{
			return i_context.m_visibility.Where(kvp => kvp.Value >= i_minVisibility).Select(kvp => kvp.Key);
		}

		public IEnumerable<Goal> EvaluateGoals(Context i_context)
		{
			var goals = m_globalGoals.GetComponentsInChildren<Goal>();

			if (c_verboseDebug)
			{
				// Debugging
				foreach (var goal in goals)
				{
					Debug.Log($"Goal \"{goal.m_description}\": " + (goal.Check(this, i_context) ? "Succeeded" : "Failed"));
				}
			}

			return goals.Where(goal => goal.Check(this, i_context));
		}

		private IEnumerator PrepareContextCoroutine(Context i_context)
		{
			yield return new WaitForEndOfFrame();

			Texture2D readableSaveTexture = RecordCameraToNewReadableTexture(i_context.m_analysisCamera);

			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();
			yield return new WaitForEndOfFrame();

			AnalysePOIs(readableSaveTexture, i_context);

			Destroy(readableSaveTexture);

			i_context.m_ready = true;
		}

		private void AnalysePOIs(Texture2D i_readableTexture, Context i_context)
		{
			var lookup = new Dictionary<int, int>();

			var pixels = i_readableTexture.GetRawTextureData<int>();
			int pixelCount = i_readableTexture.height * i_readableTexture.width;
			int stride = pixels.Length / pixelCount;
			for (int y = 0; y < i_readableTexture.height; ++y)
			{
				for (int x = 0; x < i_readableTexture.width; ++x)
				{
					var px = pixels[((y * i_readableTexture.width) + x) * stride];
					if (px != 0)
					{
						if (!lookup.ContainsKey(px))
						{
							lookup[px] = 0;
						}
						lookup[px]++;
					}
				}
			}

			i_context.m_visibility = new Dictionary<PointOfInterest, int>();
			foreach (var kvp in lookup)
			{
				var poi = PointOfInterest.FindForId(kvp.Key);
				if (poi != null)
				{
					i_context.m_visibility[poi] = kvp.Value;
					if (c_verboseDebug)
					{
						Debug.Log($"PhotoEvaluator: {kvp.Value} pixels of {poi.gameObject.name} found.");
					}
				}
				else
				{
					Debug.LogError($"PhotoEvaluator: Could not find point of interest for id {kvp.Key} ({kvp.Value} pixels)");
				}

			}
		}

		private Texture2D RecordCameraToNewReadableTexture(Camera i_camera)
		{
			int width = i_camera.targetTexture.width;
			int height = i_camera.targetTexture.height;
			Texture2D readableSaveTexture = new Texture2D(width, height, TextureFormat.RGBAFloat, false);

			Graphics.SetRenderTarget(i_camera.targetTexture);
			readableSaveTexture.ReadPixels(new Rect(0, 0, width, height), 0, 0);
			Graphics.SetRenderTarget(null);
			readableSaveTexture.Apply();

			return readableSaveTexture;
		}

		private const bool c_verboseDebug = false;
	}
}