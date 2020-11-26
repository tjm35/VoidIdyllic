using Moonshot.Photos;
using Moonshot.Planet;
using Moonshot.Props;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
				var sp = MakeSharedPrefab(go);
				var hrp = MakeHighResPrefab(sp, go.name);
				var op = MakeOrreryPrefab(sp, hrp, go.name);
				var hp = MakeOrreryHookPrefab(op, go.name);
			}
		}

		[MenuItem("GameObject/Make prop prefabs for combined", false, 0)]
		public static void MakePropPrefabForCombined(MenuCommand i_command)
		{
			foreach (var go in Selection.gameObjects)
			{
				Debug.Log($"Making prefabs for {go.name}");
				var sp = MakeSharedPrefabForCombined(go);
				var hrp = MakeHighResPrefab(sp, go.name);
				var op = MakeOrreryPrefab(sp, hrp, go.name);
				var hp = MakeOrreryHookPrefab(op, go.name);
			}
		}

		private static GameObject MakeSharedPrefab(GameObject imported)
		{
			string name = imported.name + "Shared";
			var go = InstantiateWithEmptyParent(imported, name);

			SetupRenderers(go);

			return MakePrefabAndDelete(go, imported.name);
		}

		private static GameObject MakeSharedPrefabForCombined(GameObject imported)
		{
			string name = imported.name + "Shared";
			var go = DuplicateWithEmptyParent(imported, name);

			// Record transforms.
			var instantiated = go.transform.GetChild(0);
			instantiated.SetParent(imported.transform);
			instantiated.localPosition = Vector3.zero;
			instantiated.localRotation = Quaternion.identity;
			instantiated.localScale = Vector3.one;

			var planetParent = imported.transform.GetComponentInAncestors<OrreryPlanet>();
			instantiated.SetParent((planetParent != null ? planetParent.transform : null), true);
			instantiated.SetParent(go.transform, false);

			SetupRenderers(go);
			RemoveCached(go);

			return MakePrefabAndDelete(go, imported.name);
		}

		private static void RemoveCached(GameObject go)
		{
			var allComponents = go.transform.GetComponentsInDescendents<MonoBehaviour>().ToArray();
			foreach (var c in allComponents)
			{
				if (c.name == "CachedComponents")
				{
					Component.DestroyImmediate(c);
				}
			}
		}

		private static GameObject MakeOrreryPrefab(GameObject shared, GameObject hrp, string groupName)
		{
			string name = groupName + "Orrery";
			var go = InstantiateWithEmptyParent(shared, name);

			var poi = go.AddComponent<PointOfInterest>();
			poi.m_class = PointOfInterest.Class.Artifact;

			var op = go.AddComponent<OrreryProp>();
			op.m_highResPrefab = hrp;
			op.m_pointOfInterest = poi;

			ApplySpecialMeshesToDescendents(go, false);

			return MakePrefabAndDelete(go, groupName);
		}

		private static GameObject MakeOrreryHookPrefab(GameObject orreryPrefab, string groupName)
		{
			string name = groupName + "Hook";
			var go = InstantiateWithEmptyParent(orreryPrefab, name);
			go.transform.GetChild(0).localPosition = 10.0f * Vector3.up;

			return MakePrefabAndDelete(go, groupName);
		}

		private static GameObject MakeHighResPrefab(GameObject shared, string groupName)
		{
			string name = groupName + "HighRes";
			var go = InstantiateWithEmptyParent(shared, name);

			go.AddComponent<HighResProp>();

			go.AddComponent<POIReference>();

			ApplySpecialMeshesToDescendents(go, true);

			return MakePrefabAndDelete(go, groupName);
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

		private static GameObject DuplicateWithEmptyParent(GameObject imported, string parentName)
		{
			var go = new GameObject(parentName);
			var placed = GameObject.Instantiate(imported);
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