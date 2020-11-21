using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class PointOfInterest : MonoBehaviour, IPOIContext
	{
		[System.Flags]
		public enum Class
		{
			None = 0,
			CelestialBody = 1,
			Planet = 2 | CelestialBody,
			Moon = 4 | CelestialBody,
			Sun = 8 | CelestialBody,
			Artifact = 16,
		}

		public Class m_class = Class.None;

		public bool IsOfClass(Class i_class) => (m_class & i_class) == i_class;

		public void Start()
		{
			m_id = s_intGenerator.GetNextInt();
			if (s_dictionary.ContainsKey(m_id))
			{
				Debug.LogError($"PointOfInterest: Duplicate id {m_id}");
			}
			s_dictionary[m_id] = this;
		}

		public void OnDestroy()
		{
			s_dictionary[m_id] = null;
		}

		public void SetupMaterial(MaterialPropertyBlock i_block)
		{
			i_block.SetInt("_WriteInt", m_id);
		}

		public static PointOfInterest FindForId(int i_id)
		{
			if (s_dictionary.TryGetValue(i_id, out var poi))
			{
				return poi;
			}
			else
			{
				return null;
			}
		}

		private class IntGenerator
		{
			private int m_id = 1;

			public int GetNextInt()
			{
				if (m_id == int.MaxValue)
				{
					m_id = 1;
				}
				return m_id++;
			}
		}

		private int m_id = 0;

		private static Dictionary<int, PointOfInterest> s_dictionary = new Dictionary<int, PointOfInterest>();
		private static IntGenerator s_intGenerator = new IntGenerator();
	}
}