using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.UI
{
	public class UIColors : MonoBehaviour
	{
		public interface Settable
		{
			void SetupColors(UIColors i_colors);
		}

		public enum ColorType
		{
			Primary,
			Background,
			ReversePrimary,
			Borders,
			Selected,
			Pressed,
			Alarm,
		}

		[Serializable]
		public class Palette
		{
			public Color Primary = Color.white;
			public Color Background = Color.black;
			public Color ReversePrimary = Color.black;
			public Color Borders = Color.white;
			public Color Selected = Color.white;
			public Color Pressed = Color.gray;
			public Color Alarm = Color.red;
		}

		public Palette m_palette;

		public Color Get(ColorType i_type)
		{
			switch (i_type)
			{
				case ColorType.Primary:
					return m_palette.Primary;
				case ColorType.Background:
					return m_palette.Background;
				case ColorType.ReversePrimary:
					return m_palette.ReversePrimary;
				case ColorType.Borders:
					return m_palette.Borders;
				case ColorType.Selected:
					return m_palette.Selected;
				case ColorType.Pressed:
					return m_palette.Pressed;
				case ColorType.Alarm:
					return m_palette.Alarm;
			}
			return new Color(1.0f, 0.6f, 0.8f);
		}

		public void SetPalette(Palette i_newPalette)
		{
			m_palette = i_newPalette;
			UpdateColors();
		}

		[ContextMenu("Update Colors")]
		public void UpdateColors()
		{
			foreach (var settable in transform.GetComponentsInDescendents<Settable>())
			{
				settable.SetupColors(this);
			}
		}
	}
}