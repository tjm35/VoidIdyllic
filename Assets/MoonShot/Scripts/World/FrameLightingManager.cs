using Moonshot.Planet;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Moonshot.World
{
	public class FrameLightingManager : MonoBehaviour
	{
		public static FrameLightingManager Instance;

		public List<OrreryPlanet> m_suns;
		public float m_lightBlendRate = 0.5f;

		public void EnterPlanetFrame(OrreryPlanet i_planet, LocalFrame i_newFrame)
		{
			UpdateLightFrames(i_newFrame);
			SetAllLightsToDecay();
			foreach (OrreryPlanet planet in i_planet.LightSources)
			{
				var lightData = EnsurePlanetLightForLocalFrame(planet);
				lightData.m_targetWeight = 1.0f;
			}

			// TODO: This might be better handled with ambient or even hemispherical lighting.
			var selfLightData = EnsurePlanetSelfLightForLocalFrame(i_planet);
			selfLightData.m_targetWeight = 1.0f;
		}

		private void UpdateLightFrames(LocalFrame i_newFrame)
		{
			foreach (var light in AllLights)
			{
				light.m_lightObject.GetComponent<CrossFramePositionConstraint>().m_overrideOwnFrame = i_newFrame;
			}
		}

		private LightData EnsurePlanetLightForLocalFrame(OrreryPlanet planet)
		{
			if (m_lights.TryGetValue(planet, out var lightData))
			{
				return lightData;
			}
			else
			{
				return CreatePlanetLightForPlanet(planet, false);
			}
		}

		private LightData EnsurePlanetSelfLightForLocalFrame(OrreryPlanet planet)
		{
			if (m_selfLights.TryGetValue(planet, out var lightData))
			{
				return lightData;
			}
			else
			{
				return CreatePlanetLightForPlanet(planet, true);
			}
		}

		private LightData CreatePlanetLightForPlanet(OrreryPlanet planet, bool isSelfLight)
		{
			LightData newLightData = new LightData();
			newLightData.m_sourceBody = planet;
			newLightData.m_lightObject = planet.SetupLightForLocalFrames(transform);
			newLightData.m_baseIntensity = planet.PlanetIntensity;
			newLightData.m_weight = 0.0f;
			newLightData.m_targetWeight = 0.0f;
			if (isSelfLight)
			{
				Light light = newLightData.m_lightObject;
				light.type = LightType.Point;
				light.range = 2.0f * planet.Radius;
				light.gameObject.name += " (Self)";
				newLightData.m_baseIntensity *= planet.Radius * planet.Radius;
			}

			if (isSelfLight)
			{
				m_selfLights.Add(planet, newLightData);
			}
			else
			{
				m_lights.Add(planet, newLightData);
			}

			return newLightData;
		}

		private void SetAllLightsToDecay()
		{
			foreach (var light in AllLights)
			{
				light.m_targetWeight = 0.0f;
			}
		}

		public void EnterSpaceFrame(Transform i_globalLocation, LocalFrame i_newFrame)
		{
			UpdateLightFrames(i_newFrame);
			SetAllLightsToDecay();
			foreach (var sunLightData in EnsureSunLights())
			{
				sunLightData.m_targetWeight = 1.0f;
			}
			// TODO - Enable other lights if we're close enough?
		}

		private IEnumerable<LightData> EnsureSunLights()
		{
			foreach (var sun in m_suns)
			{
				yield return EnsurePlanetLightForLocalFrame(sun);
			}
		}

		private void Start()
		{
			Debug.Assert(Instance == null);
			Instance = this;
		}

		private void OnDestroy()
		{
			Debug.Assert(Instance == this);
			Instance = null;
		}

		private void Update()
		{
			foreach (var lightData in AllLights)
			{
				UpdateLightBlending(lightData);
			}
		}

		private void UpdateLightBlending(LightData lightData)
		{
			if (lightData.m_weight < lightData.m_targetWeight)
			{
				lightData.m_weight = Mathf.Min(lightData.m_targetWeight, lightData.m_weight + Time.deltaTime * m_lightBlendRate);
			}
			if (lightData.m_weight > lightData.m_targetWeight)
			{
				lightData.m_weight = Mathf.Max(lightData.m_targetWeight, lightData.m_weight - Time.deltaTime * m_lightBlendRate);
			}

			float attenuation = 1.0f; // TODO - Distance attenuation;
			lightData.m_lightObject.intensity = lightData.m_baseIntensity * attenuation * lightData.m_weight;

			lightData.m_lightObject.gameObject.SetActive(lightData.m_weight > 0.0f);
		}

		private IEnumerable<LightData> AllLights => m_lights.Values.Concat(m_selfLights.Values);

		private class LightData
		{
			public Light m_lightObject;
			public OrreryPlanet m_sourceBody;
			public float m_baseIntensity;
			public float m_weight;
			public float m_targetWeight;
		}

		private Dictionary<OrreryPlanet, LightData> m_lights = new Dictionary<OrreryPlanet, LightData>();
		private Dictionary<OrreryPlanet, LightData> m_selfLights = new Dictionary<OrreryPlanet, LightData>();
	}
}