using System;
using System.Collections;
using System.Collections.Generic;
using Moonshot.World;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

namespace Moonshot.Photos
{
	public class PhotoEvaluator : MonoBehaviour
	{
		public Transform m_globalGoals;
		public bool m_verboseDebug = false;
		public bool m_backgroundEvaluation = true;
		public Camera m_backgroundEvaluationCamera;
		public int m_backgroundEvaluationVisibilityThreshold = 1;
		public ComputeShader m_analysisRecordingShader;
		public Texture POIAnalysisTexture;

		public class Context
		{
			public Transform m_globalLocation;
			public Dictionary<PointOfInterest, int> m_visibility;
			public Vector3 m_globalCameraPos;
			public bool m_ready;
		}

		private void Start()
		{
			m_analysisOutputBuffer = new ComputeBuffer(POIAnalysisTexture.width * POIAnalysisTexture.height, 4);
		}

		private void Update()
		{
			if (m_backgroundEvaluation)
			{
				if (m_backgroundContext == null)
				{
					if (m_backgroundEvaluationCamera.isActiveAndEnabled)
					{
						m_backgroundContext = ConstructContext(m_backgroundEvaluationCamera);
					}
					else
					{
						foreach (var poi in m_backgroundVisiblePoIs)
						{
							if (poi)
							{
								poi.SendMessage("OnExitedViewfinder", poi, SendMessageOptions.DontRequireReceiver);
							}
						}
						m_backgroundVisiblePoIs.Clear();
					}
				}
				if (m_backgroundContext != null && m_backgroundContext.m_ready)
				{
					var visiblePoIs = GetVisiblePOIs(m_backgroundEvaluationVisibilityThreshold, m_backgroundContext).ToArray();
					foreach (var poi in visiblePoIs)
					{
						if (!m_backgroundVisiblePoIs.Contains(poi))
						{
							poi.SendMessage("OnEnteredViewfinder", poi, SendMessageOptions.DontRequireReceiver);
							m_backgroundVisiblePoIs.Add(poi);
						}
					}
					var deadList = new List<PointOfInterest>();
					foreach (var poi in m_backgroundVisiblePoIs)
					{
						if (poi && !visiblePoIs.Contains(poi))
						{
							poi.SendMessage("OnExitedViewfinder", poi, SendMessageOptions.DontRequireReceiver);
							deadList.Add(poi);
						}
					}
					foreach (var poi in deadList)
					{
						m_backgroundVisiblePoIs.Remove(poi);
					}
					m_backgroundVisiblePoIs.Remove(null);

					m_backgroundContext = null;
				}
			}
		}

		private class BuildingContext : Context
		{ 
			public Camera m_analysisCamera;
		}

		public Context ConstructContext(Camera i_viewCamera)
		{
			var context = new BuildingContext();
			context.m_analysisCamera = i_viewCamera.GetComponent<POIAnalysisCameraRef>().m_poiAnalysisCamera;
			var lf = LocalFrame.Get(i_viewCamera.transform);
			if (lf)
			{
				context.m_globalLocation = lf.GlobalLocation;
			}

			// Camera should be positioned in global frame, so we do *not* need to frame transform it.
			Debug.Assert(i_viewCamera.GetComponent<PositionInGlobalFrame>());
			context.m_globalCameraPos = i_viewCamera.transform.position;

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

		public Vector3 GetGlobalPOIPosition(PointOfInterest i_poi, Context i_context)
		{
			// TODO - Cache positions in the context since they might have changed since the photo was taken.
			// For now just return the current positiion.
			return LocalFrame.GetGlobalPosition(i_poi.transform);
		}

		public IEnumerable<Goal> EvaluateGoals(Context i_context)
		{
			var goals = m_globalGoals.GetComponentsInChildren<Goal>().Where(g => g.isActiveAndEnabled);

			if (m_verboseDebug)
			{
				// Debugging
				foreach (var goal in goals)
				{
					Debug.Log($"Goal \"{goal.m_description}\": " + (goal.Check(this, i_context) ? "Succeeded" : "Failed"));
				}
			}

			return goals.Where(goal => goal.Check(this, i_context));
		}

		public void NotifyPhotoTaken(Context i_context)
		{
			foreach (var poi in GetVisiblePOIs(m_backgroundEvaluationVisibilityThreshold, i_context))
			{
				poi.SendMessage("OnPhotoTaken", poi, SendMessageOptions.DontRequireReceiver);
			}
		}

		private IEnumerator PrepareContextCoroutine(BuildingContext i_context)
		{
			while (m_computeBufferInUse)
			{
				yield return new WaitForEndOfFrame();
			}

			m_analysisRecordingShader.SetTexture(0, "AnalysisImage", i_context.m_analysisCamera.targetTexture);
			m_analysisRecordingShader.SetBuffer(0, "AnalysisImageBuffer", m_analysisOutputBuffer);
			m_analysisRecordingShader.Dispatch(0, i_context.m_analysisCamera.targetTexture.width / 8, i_context.m_analysisCamera.targetTexture.height / 8, 1);
			m_computeBufferInUse = true;

			var readbackRequest = AsyncGPUReadback.Request(m_analysisOutputBuffer);

			while (!readbackRequest.done && !readbackRequest.hasError)
			{
				yield return new WaitForEndOfFrame();
			}

			if (readbackRequest.hasError == false)
			{
				AnalysePOIs(readbackRequest, i_context);
			}

			m_computeBufferInUse = false;

			i_context.m_ready = true;
		}

		private void AnalysePOIs(AsyncGPUReadbackRequest i_readbackRequest, Context i_context)
		{
			var lookup = new Dictionary<int, int>();

			var pixels = i_readbackRequest.GetData<int>();
			for (int i = 0; i < pixels.Length; ++i)
			{
				var px = pixels[i];
				if (px != 0)
				{
					if (!lookup.ContainsKey(px))
					{
						lookup[px] = 0;
					}
					lookup[px]++;
				}
			}

			i_context.m_visibility = new Dictionary<PointOfInterest, int>();
			foreach (var kvp in lookup)
			{
				var poi = PointOfInterest.FindForId(kvp.Key);
				if (poi != null)
				{
					i_context.m_visibility[poi] = kvp.Value;
					if (m_verboseDebug)
					{
						UI.QuickDebug.Print($"PhotoEvaluator: {kvp.Value} pixels of {poi.gameObject.name} found.");
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

		private Context m_backgroundContext;
		private HashSet<PointOfInterest> m_backgroundVisiblePoIs = new HashSet<PointOfInterest>();
		private ComputeBuffer m_analysisOutputBuffer;
		private bool m_computeBufferInUse = false;
	}
}