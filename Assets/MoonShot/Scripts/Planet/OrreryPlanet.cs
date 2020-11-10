using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.World;
using Moonshot.Props;
using System;
using UnityEngine.Animations;

namespace Moonshot.Planet
{
	public class OrreryPlanet : MonoBehaviour
	{
		public Color PlanetColor = Color.white;
		public float PlanetIntensity = 1;
		public float Radius = 100.0f;

		public GameObject HighResPlanetPrefab;
		public GameObject LightPrefab;

		public List<OrreryPlanet> LightSources;

		public List<OrreryProp> OrreryProps;

		public bool CanConstructFrame => HighResPlanetPrefab != null;

		public LocalFrame ConstructLocalFrame()
		{
			if (!CanConstructFrame)
			{
				return null;
			}

			GameObject frameObject = CreateObject(gameObject.name + " Local Frame", null);
			var frame = frameObject.AddComponent<LocalFrame>();
			frame.GlobalLocation = transform;

			CreateHighResPlanet(frame);
			SetupLighting(frame);
			PopulateObjects(frame);

			return frame;
		}

		[ContextMenu("Rebuild Prop List")]
		private void RebuildPropList()
		{
			OrreryProps.Clear();
			OrreryProps.AddRange(transform.GetComponentsInDescendents<OrreryProp>());
		}

		private void CreateHighResPlanet(LocalFrame frame)
		{
			Instantiate(HighResPlanetPrefab, frame.transform, false);

			// TODO - Initialise high res planet
		}

		private void SetupLighting(LocalFrame frame)
		{
			foreach (var planet in LightSources)
			{
				planet.SetupLightForOtherLocalFrame(frame);
			}
		}

		private void PopulateObjects(LocalFrame frame)
		{
			foreach (var prop in OrreryProps)
			{
				if (prop != null)
				{
					prop.CreateHighResProp(frame);
				}
			}
		}

		private void SetupLightForOtherLocalFrame(LocalFrame frame)
		{
			GameObject lightObject = Instantiate(LightPrefab, frame.transform, false);
			lightObject.name = gameObject.name + " Light";

			Light light = lightObject.GetComponent<Light>();
			light.color = PlanetColor;
			light.intensity = PlanetIntensity; // TODO: Intensity falloff by distance

			LookAtConstraint lac = lightObject.GetComponent<LookAtConstraint>();
			if (lac == null)
			{
				lac = lightObject.AddComponent<LookAtConstraint>();
			}
			ConstraintSource cs = new ConstraintSource();
			cs.sourceTransform = frame.transform;
			cs.weight = 1.0f;
			lac.AddSource(cs);
			lac.constraintActive = true;

			CrossFramePositionConstraint cfpc = lightObject.GetComponent<CrossFramePositionConstraint>();
			if (cfpc == null)
			{
				cfpc = lightObject.AddComponent<CrossFramePositionConstraint>();
			}
			cfpc.m_target = transform;
		}

		private GameObject CreateObject(string i_name, Transform i_parent)
		{
			var obj = new GameObject(i_name);
			if (i_parent)
			{
				obj.transform.SetParent(i_parent);
			}
			obj.transform.localPosition = Vector3.zero;
			obj.transform.localRotation = Quaternion.identity;
			obj.transform.localScale = Vector3.one;
			return obj;
		}

	}
}