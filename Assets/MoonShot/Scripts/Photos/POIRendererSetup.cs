using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	[RequireComponent(typeof(Renderer))]
	public class POIRendererSetup : MonoBehaviour
	{
		public bool m_everyFrame = false;

		void Start()
		{
			m_renderer = GetComponent<Renderer>();
		}

		void Update()
		{
			if (m_poi == null)
			{
				m_poi = transform.GetComponentInAncestors<IPOIContext>();
				m_poiObject = m_poi as Object;
				if (m_poi == null && Application.isPlaying)
				{
					Debug.LogError("POIRendererSetup: No POI in ancestors; disabling.");
					enabled = false;
				}
			}

			if (m_poi != null)
			{ 
				if (m_block == null)
				{
					m_block = new MaterialPropertyBlock();
					m_poi.SetupMaterial(m_block);
				}

				if (m_everyFrame)
				{
					m_poi.SetupMaterial(m_block);
				}

				m_renderer.SetPropertyBlock(m_block);
			}
		}

		[ReadOnly]
		[SerializeField]
		private Object m_poiObject;

		private IPOIContext m_poi = null;
		private Renderer m_renderer = null;
		private MaterialPropertyBlock m_block = null;
	}
}