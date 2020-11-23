using Moonshot.Planet;
using Moonshot.Player;
using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Moonshot.World
{
	public class PostProcessingManager : MonoBehaviour
	{
		[Serializable]
		public class PlanetaryProcessingSetup
		{
			public OrreryPlanet m_planet;
			public Volume m_volume;
		}

		[ReorderableList]
		public List<PlanetaryProcessingSetup> m_planetProcessing;
		public float m_fadeInTime = 3.0f;
		public float m_fadeOutTime = 3.0f;

		void Update()
		{
			if (m_current != GetSetupForCurrentFramePlanet())
			{
				if (m_current != null)
				{
					m_current.m_volume.weight = Mathf.Max(m_current.m_volume.weight - Time.deltaTime / m_fadeInTime, 0.0f);
					if (m_current.m_volume.weight == 0.0f)
					{
						m_current = null;
					}
				}

				if (m_current == null)
				{
					m_current = GetSetupForCurrentFramePlanet();
				}
			}
			else
			{
				if (m_current != null)
				{
					m_current.m_volume.weight = Mathf.Min(m_current.m_volume.weight + Time.deltaTime / m_fadeInTime, 1.0f);
				}
			}
		}

		private PlanetaryProcessingSetup GetSetupForCurrentFramePlanet()
		{
			if (PlayerVehicle.Current)
			{
				LocalFrame lf = LocalFrame.Get(PlayerVehicle.Current.transform);
				if (lf)
				{
					OrreryPlanet currentPlanet = lf.GlobalLocation.GetComponent<OrreryPlanet>();
					return m_planetProcessing.Find(pm => pm.m_planet == currentPlanet);
				}
			}
			return null;
		}

		private PlanetaryProcessingSetup m_current;

	}
}