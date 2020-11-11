using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.World;
using Moonshot.Props;
using System;
using UnityEngine.Animations;

namespace Moonshot.Planet
{
	public class OrreryPlanet : MonoBehaviour, PlanetMeshTools.IMeshBuildContext
	{
		public Color PlanetColor = Color.white;
		public float PlanetIntensity = 1;
		public float Radius = 100.0f;
		public Material SurfaceMaterial;

		public GameObject HighResPlanetPrefab;
		public GameObject LightPrefab;

		public MeshFilter OrreryMeshFilter;
		public int OrrerySubDivs = 4;
		public float OrreryMeshSmoothness = 0.0f;

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
			var go = Instantiate(HighResPlanetPrefab, frame.transform, false);

			var hrp = go.GetComponent<HighResPlanet>();

			hrp.OrreryPlanet = this;
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

		public void NudgeDirection(ref Vector3 io_direction)
		{
			// Do nothing
		}

		public float GetRadiusInDirection(Vector3 i_direction)
		{
			float radius = Radius;
			//radius *= (1.0f + 0.02f * Mathf.Sin(6.0f * i_direction.y));
			AdjustRadiusForCutaways(ref radius, i_direction);
			return radius;
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
	}
}