
using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class VaryDetails : VaryBase
	{
		[Required]
		public GameObject m_variationObject;
		[ReorderableList]
		public Transform[] m_parentCandidates;
		public float m_baseSpawnProbability = 0.0f;
		public float m_spawnProbailityWithScale = 0.5f;
		public float m_subsequentSpawnProbailityMultiplier = 0.0f;
		public int m_maxSpawns = 3;
		public Vector3 m_spawnHalfSize = Vector3.one;
		public Vector3 m_spawnOffset = Vector3.zero;
		public float m_minScale = 0.9f;
		public float m_maxScale = 1.1f;
		public List<GameObject> m_spawnedObjects = new List<GameObject>();

		[Button]
		public override void ApplyVariation()
		{
			RemoveOldObjects();
			int spawned = 0;

			var rand = MakeRandomiser();

			while (spawned < m_maxSpawns)
			{
				float probability = m_baseSpawnProbability + m_spawnProbailityWithScale * transform.lossyScale.x;
				if (spawned > 0)
				{
					probability *= Mathf.Pow(m_subsequentSpawnProbailityMultiplier, (float)spawned);
				}

				if (rand.NextDouble() < probability)
				{
					SpawnOne(rand);
					spawned++;
				}
				else
				{
					break;
				}
			}
		}

		private void RemoveOldObjects()
		{
			foreach (var go in m_spawnedObjects)
			{
				DestroyImmediate(go);
			}
			m_spawnedObjects.Clear();
		}

		private void SpawnOne(System.Random i_rand)
		{
			Transform parent = m_parentCandidates[RandInt(i_rand, 0, m_parentCandidates.Length)];
			Vector3 pos = RandInBox(i_rand, m_spawnHalfSize) + m_spawnOffset;
			float scale = Rand(i_rand, m_minScale, m_maxScale);

			var spawned = Instantiate(m_variationObject, transform);
			spawned.transform.localPosition = transform.InverseTransformPoint(parent.position) + pos;
			spawned.transform.localRotation = Quaternion.identity;
			spawned.transform.localScale = scale * Vector3.one;
			spawned.SetActive(true);

			m_spawnedObjects.Add(spawned);
		}
	}
}