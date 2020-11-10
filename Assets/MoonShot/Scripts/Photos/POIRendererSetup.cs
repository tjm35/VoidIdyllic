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
				Transform t = transform;
				while (t != null && t.GetComponent<IPOIContext>() == null)
				{
					t = t.parent;
				}
				if (t != null)
				{
					m_poi = t.GetComponent<IPOIContext>();
				}
				if (m_poi == null)
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