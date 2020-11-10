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

		public void Start()
		{
			m_color = s_colorGenerator.GetNextColor();
			if (s_dictionary.ContainsKey(m_color))
			{
				Debug.LogError($"PointOfInterest: Duplicate color {m_color}");
			}
			s_dictionary[m_color] = this;
		}

		public void OnDestroy()
		{
			s_dictionary[m_color] = null;
		}

		public void SetupMaterial(MaterialPropertyBlock i_block)
		{
			i_block.SetColor("WriteColor", m_color);
		}

		public static PointOfInterest FindForColor(Color32 i_color)
		{
			if (s_dictionary.TryGetValue(i_color, out var poi))
			{
				return poi;
			}
			else
			{
				return null;
			}
		}

		private class ColorGenerator
		{
			public Color32 GetNextColor()
			{
				if (m_colorID < m_colors.Length)
				{
					return m_colors[m_colorID++];
				}
				else
				{
					Debug.LogError("PointOfInterest.ColorGenerator: Out of colours");
					return new Color32(0, 0, 0, 0);
				}
			}

			private int m_colorID = 0;

			private Color32[] m_colors = new Color32[]
			{
				new Color32(255,0,0,255),
				new Color32(0,255,0,255),
				new Color32(255,255,0,255),
				new Color32(0,0,255,255),
				new Color32(255,0,255,255),
				new Color32(0,255,255,255),
				new Color32(255,255,255,255),
			};
		}

		private Color32 m_color = Color.white;

		private static Dictionary<Color, PointOfInterest> s_dictionary = new Dictionary<Color, PointOfInterest>();
		private static ColorGenerator s_colorGenerator = new ColorGenerator();
	}
}