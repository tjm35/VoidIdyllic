using Moonshot.Player;
using Moonshot.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Planet
{
	[RequireComponent(typeof(Renderer))]
	public class CloudSurfaceParamSetup : MonoBehaviour
	{
		public float m_radius = 40.0f;

		// Start is called before the first frame update
		void Start()
		{
			m_renderer = GetComponent<Renderer>();
			m_clipSphereCenterID = Shader.PropertyToID("_ClipSphereCenter");
			m_clipSphereRadiusID = Shader.PropertyToID("_ClipSphereRadius");
		}

		// Update is called once per frame
		void Update()
		{
			if (m_block == null)
			{
				m_block = new MaterialPropertyBlock();
			}

			if (PlayerVehicle.Current)
			{
				LocalFrame lf = LocalFrame.Get(transform);
				m_block.SetVector(m_clipSphereCenterID, transform.InverseTransformPoint(LocalFrame.TransformPointToLocal(lf, LocalFrame.GetGlobalPosition(PlayerVehicle.Current.transform))));
				m_block.SetFloat(m_clipSphereRadiusID, m_radius);
			}
			else
			{
				m_block.SetFloat(m_clipSphereRadiusID, 0.0f);
			}
			m_renderer.SetPropertyBlock(m_block);
		}

		private Renderer m_renderer;
		private MaterialPropertyBlock m_block = null;
		private int m_clipSphereCenterID;
		private int m_clipSphereRadiusID;

	}
}