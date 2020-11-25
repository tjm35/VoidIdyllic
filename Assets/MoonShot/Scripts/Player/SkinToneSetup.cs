using Moonshot.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Player
{
	[RequireComponent(typeof(Renderer))]
	public class SkinToneSetup : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			m_block = new MaterialPropertyBlock();
			m_renderer = GetComponent<Renderer>();
			m_baseColorID = Shader.PropertyToID("_BaseColor");
			m_emissiveColorID = Shader.PropertyToID("_EmissionColor");
			m_metallicID = Shader.PropertyToID("_Metallic");
			m_smoothnessID = Shader.PropertyToID("_Smoothness");
		}

		// Update is called once per frame
		void Update()
		{
			var tone = SkinToneSetting.Tone;
			if (tone != null)
			{
				m_block.SetColor(m_baseColorID, tone.m_primaryColor);
				m_block.SetColor(m_emissiveColorID, tone.m_emissiveColor);
				m_block.SetFloat(m_metallicID, tone.m_metallic);
				m_block.SetFloat(m_smoothnessID, tone.m_smoothness);
				m_renderer.SetPropertyBlock(m_block);
			}
		}

		private Renderer m_renderer;
		private MaterialPropertyBlock m_block;
		private int m_baseColorID;
		private int m_emissiveColorID;
		private int m_metallicID;
		private int m_smoothnessID;
	}
}