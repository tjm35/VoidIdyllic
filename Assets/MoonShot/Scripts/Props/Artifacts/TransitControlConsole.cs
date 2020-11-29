using Moonshot.Photos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class TransitControlConsole : MonoBehaviour
	{
		public Renderer m_iconRenderer;
		public TransitCore m_core;
		public int m_destID = 0;

		public void OnPhotoTaken(PointOfInterest i_poi)
		{
			m_core.Activate(m_destID);
		}

		private void Start()
		{
			m_isActiveID = Shader.PropertyToID("_IsActive");
		}

		private void Update()
		{
			if (m_iconRenderer)
			{
				m_iconRenderer.material.SetInt(m_isActiveID, m_core.IsActive(m_destID) ? 1 : 0);
			}
		}

		private int m_isActiveID;
	}
}