using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.Photos;

namespace Moonshot.Planet
{
	public class HighResPlanet : MonoBehaviour
	{
		public MeshFilter m_highResMeshFilter;
		public MeshCollider m_highResMeshCollider;

		public OrreryPlanet OrreryPlanet
		{
			get { return m_orreryPlanet; }
			set { m_orreryPlanet = value; OnPlanetSet(); }
		}

		private void OnPlanetSet()
		{
			// For now just share the orrery mesh.
			if (m_highResMeshFilter)
			{
				m_highResMeshFilter.sharedMesh = OrreryPlanet.OrreryMeshFilter.sharedMesh;
				var mr = m_highResMeshFilter.GetComponent<MeshRenderer>();
				mr.sharedMaterial = OrreryPlanet.SurfaceMaterial;
			}
			if (m_highResMeshCollider)
			{
				m_highResMeshCollider.sharedMesh = OrreryPlanet.OrreryMeshFilter.sharedMesh;
			}
			if (GetComponent<POIReference>())
			{
				GetComponent<POIReference>().m_referencedPOI = OrreryPlanet.m_pointOfInterest;
			}

		}

		private OrreryPlanet m_orreryPlanet;
	}
}