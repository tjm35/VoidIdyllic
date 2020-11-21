using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.World;
using Moonshot.Photos;
using Moonshot.Props;
using System;
using System.Linq;
using UnityEngine.Animations;

namespace Moonshot.Planet
{
	public class OrreryPlanet : MonoBehaviour, PlanetMeshTools.IMeshBuildContext
	{
		public Color PlanetColor = Color.white;
		public float PlanetIntensity = 1;
		public float Radius = 100.0f;
		public Material SurfaceMaterial;
		public Cubemap HeightMap;
		public float HeightMapScale = 10.0f;

		public GameObject HighResPlanetPrefab;
		public GameObject LightPrefab;

		public MeshFilter OrreryMeshFilter;
		public int OrrerySubDivs = 4;
		public float OrreryMeshSmoothness = 0.0f;
		public PointOfInterest m_pointOfInterest;

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
			PopulateObjects(frame);

			return frame;
		}

		public Light SetupLightForLocalFrames(Transform parent)
		{
			GameObject lightObject = Instantiate(LightPrefab, parent, false);
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
			cs.sourceTransform = parent;
			cs.weight = 1.0f;
			lac.AddSource(cs);
			lac.constraintActive = true;

			CrossFramePositionConstraint cfpc = lightObject.GetComponent<CrossFramePositionConstraint>();
			if (cfpc == null)
			{
				cfpc = lightObject.AddComponent<CrossFramePositionConstraint>();
			}
			cfpc.m_target = transform;

			return light;
		}

		[ContextMenu("Rebuild Prop List")]
		private void RebuildPropList()
		{
			OrreryProps.Clear();
			OrreryProps.AddRange(transform.GetComponentsInDescendents<OrreryProp>().Where(p => p.transform.GetComponentInAncestors<OrreryPlanet>() == this));
		}

		[ContextMenu("Rebuild Orrery Lighting")]
		private void RebuildOrreryLighting()
		{
			var lightContainer = transform.Find("OrreryLights");
			if (lightContainer == null)
			{
				lightContainer = CreateObject("OrreryLights",transform).transform;
				lightContainer.SetParent(transform);
			}
			while (lightContainer.childCount > 0)
			{
				var oldLight = lightContainer.GetChild(0);
				oldLight.SetParent(null);
				DestroyImmediate(oldLight.gameObject);
			}
			foreach (var planet in LightSources)
			{
				planet.SetupLightForOrrery(this, lightContainer);
			}
			SetupSelfLightForOrrery(lightContainer);
		}


		private void CreateHighResPlanet(LocalFrame frame)
		{
			var go = Instantiate(HighResPlanetPrefab, frame.transform, false);

			var hrp = go.GetComponent<HighResPlanet>();

			hrp.OrreryPlanet = this;
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

		private void SetupSelfLightForOrrery(Transform i_lightContainer)
		{
			GameObject lightObject = CreateObject(gameObject.name + " Self Light", i_lightContainer.transform);

			Light light = lightObject.AddComponent<Light>();
			light.type = LightType.Point;
			light.range = 2.0f * Radius;
			light.color = PlanetColor;
			light.intensity = PlanetIntensity * Radius * Radius;
			light.shadows = LightShadows.None;
			light.renderMode = LightRenderMode.ForceVertex;
			light.cullingMask = 1 << gameObject.layer;
		}

		private void SetupLightForOrrery(OrreryPlanet i_litPlanet, Transform i_lightContainer)
		{
			GameObject lightObject = CreateObject(gameObject.name + " Light", i_lightContainer.transform);

			Light light = lightObject.AddComponent<Light>();
			light.type = LightType.Directional;
			light.color = PlanetColor;
			light.intensity = PlanetIntensity; // TODO: Intensity falloff by distance
			light.shadows = LightShadows.None;
			light.renderMode = LightRenderMode.Auto;
			light.cullingMask = 1 << i_lightContainer.gameObject.layer;

			PositionConstraint pc = lightObject.AddComponent<PositionConstraint>();
			ConstraintSource pcs = new ConstraintSource();
			pcs.sourceTransform = transform;
			pcs.weight = 1.0f;
			pc.AddSource(pcs);
			pc.translationOffset = Vector3.zero;
			pc.locked = true;
			pc.constraintActive = true;

			LookAtConstraint lac = lightObject.AddComponent<LookAtConstraint>();
			ConstraintSource lacs = new ConstraintSource();
			lacs.sourceTransform = i_litPlanet.transform;
			lacs.weight = 1.0f;
			lac.AddSource(lacs);
			lac.constraintActive = true;
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
			if (i_parent)
			{
				obj.layer = i_parent.gameObject.layer;
			}
			else
			{
				obj.layer = gameObject.layer;
			}
			return obj;
		}

		public void NudgeDirection(ref Vector3 io_direction)
		{
			// Do nothing
		}

		public float GetRadiusInDirection(Vector3 i_direction)
		{
			float radius = Radius;
			//radius *= (1.0f + 0.02f * Mathf.Sin(6.0f * i_direction.y));
			AdjustRadiusForHeightMap(ref radius, i_direction);
			AdjustRadiusForCutaways(ref radius, i_direction);
			return radius;
		}

		private void AdjustRadiusForHeightMap(ref float io_radius, Vector3 i_direction)
		{
			if (HeightMap)
			{
				float mapValue = CubemapSampler.SampleCubemap(HeightMap, i_direction.normalized).r;
				float snormMapValue = 2.0f * (mapValue - 0.5f);

				io_radius += snormMapValue * HeightMapScale;
			}
		}

		private bool AdjustRadiusForCutaways(ref float io_radius, Vector3 i_direction)
		{
			if (Physics.Raycast(transform.TransformPoint(Vector3.zero), transform.TransformDirection(i_direction), out RaycastHit hitInfo, io_radius, LayerMask.GetMask("PlanetCutaway"), QueryTriggerInteraction.Collide))
			{
				io_radius = hitInfo.distance;
				return true;
			}
			return false;
		}

		public float SmoothNormals(Vector3 i_direction)
		{
			return OrreryMeshSmoothness;
		}

		[ContextMenu("Rebuild Orrery Mesh")]
		private void RebuildOrreryMesh()
		{
			OrreryMeshFilter.sharedMesh = PlanetMeshTools.BuildFullPlanetMesh(this, i_subdivs: OrrerySubDivs, i_name: gameObject.name + " Orrery Mesh");
		}

		private void Start()
		{
			if (m_pointOfInterest == null)
			{
				m_pointOfInterest = GetComponent<PointOfInterest>();
			}
		}
	}
}