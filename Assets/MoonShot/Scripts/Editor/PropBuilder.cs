using Moonshot.Photos;
using Moonshot.Props;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Totality;
using UnityEditor;
using UnityEngine;

namespace Moonshot.Editor
{
	public static class PropBuilder
	{
		[MenuItem("Assets/Make Prop Prefabs")]
		public static void MakePropPrefab(MenuCommand i_command)
		{
			foreach (var go in Selection.gameObjects)
			{
				Debug.Log($"Making prefabs for {go.name}");
				var hrp = MakeHighResPrefab(go);
				MakeOrreryPrefab(go, hrp);
			}
		}

		private static GameObject MakeOrreryPrefab(GameObject imported, GameObject hrp)
		{
			string name = imported.name + "Orrery";
			var go = InstantiateWithEmptyParent(imported, name);

			var poi = go.AddComponent<PointOfInterest>();
			poi.m_class = PointOfInterest.Class.Artifact;

			var op = go.AddComponent<OrreryProp>();
			op.m_highResPrefab = hrp;
			op.m_pointOfInterest = poi;

			SetupRenderers(go);
			ApplySpecialMeshesToDescendents(go, false);

			return MakePrefabAndDelete(go, imported.name);
		}

		private static GameObject MakeHighResPrefab(GameObject imported)
		{
			string name = imported.name + "HighRes";
			var go = InstantiateWithEmptyParent(imported, name);

			go.AddComponent<HighResProp>();

			go.AddComponent<POIReference>();

			SetupRenderers(go);
			ApplySpecialMeshesToDescendents(go, true);

			return MakePrefabAndDelete(go, imported.name);
		}

		private static void SetupRenderers(GameObject i_object)
		{
			foreach (var renderer in i_object.transform.GetComponentsInDescendents<Renderer>())
			{
				renderer.gameObject.AddComponent<POIRendererSetup>();
			}
		}

		private static void ApplySpecialMeshesToDescendents(GameObject i_object, bool i_highRes)
		{
			foreach (var t in i_object.transform.GetDescendents())
			{
				ApplySpecialMeshes(t.gameObject, i_highRes);
			}
		}

		private static void ApplySpecialMeshes(GameObject i_object, bool i_highRes)
		{
			if (i_object.name.Contains("Cutaway"))
			{
				i_object.layer = LayerMask.NameToLayer("PlanetCutaway");
				var mr = i_object.GetComponent<MeshRenderer>();
				if (mr)
				{
					mr.enabled = false;
				}
			}
			else
			{
				var collider = i_object.GetComponent<Collider>();
				if (collider && !i_highRes)
				{
					collider.enabled = false;
				}
			}
		}

		private static GameObject InstantiateWithEmptyParent(GameObject imported, string parentName)
		{
			var go = new GameObject(parentName);
			var placed = PrefabUtility.InstantiatePrefab(imported);
			var placedTransform = ((GameObject)placed).transform;
			placedTransform.SetParent(go.transform);
			return go;
		}

		private static GameObject MakePrefabAndDelete(GameObject i_object, string folderName)
		{
			string directory = "Assets/Moonshot/Prefabs/Artifacts/" + folderName + "/";
			Directory.CreateDirectory(directory);
			string path = directory + i_object.name + ".prefab";
			var prefab = PrefabUtility.SaveAsPrefabAsset(i_object, path);
			Debug.Log("Saved prefab to " + path);
			GameObject.DestroyImmediate(i_object);
			return prefab;
		}


	}
}