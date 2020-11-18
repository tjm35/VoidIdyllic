using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	[RequireComponent(typeof(Renderer))]
	[ExecuteAlways]
	public class POIRendererSetup : MonoBehaviour
	{
		void Start()
		{
			m_renderer = GetComponent<Renderer>();
		}

		void Update()
		{
			if (m_poi == null)
			{
				m_poi = transform.GetComponentInAncestors<IPOIContext>();
				if (m_poi == null && Application.isPlaying)
				{
					Debug.LogError("POIRendererSetup: No POI in ancestors; disabling.");
					enabled = false;
				}
			}

			if (m_block == null)
			{
				m_block = new MaterialPropertyBlock();
			}

			m_renderer.SetPropertyBlock(m_block);
			m_poi?.SetupMaterial(m_block);
		}

		private IPOIContext m_poi = null;
		private Renderer m_renderer = null;
		private MaterialPropertyBlock m_block = null;
	}
}