using Moonshot.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

namespace Moonshot.World
{
	[RequireComponent(typeof(VisualEffect))]
	public class VFXLOD : MonoBehaviour
	{
		public VisualEffectAsset[] m_lods;
		public GameObject m_fallback;
		// Start is called before the first frame update
		void Start()
		{
			m_visualEffect = GetComponent<VisualEffect>();
		}

		// Update is called once per frame
		void Update()
		{
			int lod = GetRequestedLOD();
			if (lod < m_lods.Length)
			{
				var asset = m_lods[lod];
				if (asset != m_visualEffect.visualEffectAsset)
				{
					m_visualEffect.visualEffectAsset = asset;
				}
				m_visualEffect.enabled = true;
				if (m_fallback)
				{
					m_fallback.SetActive(false);
				}
			}
			else
			{
				m_visualEffect.enabled = false;
				if (m_fallback)
				{
					m_fallback.SetActive(true);
				}
			}
		}

		private int GetRequestedLOD()
		{
			return Mathf.Max(0, ParticleLODSetting.ParticleLOD);
		}

		private VisualEffect m_visualEffect;
	}
}