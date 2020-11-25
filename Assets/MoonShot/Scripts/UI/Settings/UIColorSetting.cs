using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.UI
{

	public class UIColorSetting : MonoBehaviour
	{
		public UIColors.Palette[] m_palettes;

		public float PaletteIndex
		{
			set
			{
				int index = Mathf.Clamp(Mathf.RoundToInt(value), 0, m_palettes.Length - 1);
				transform.GetComponentInAncestors<UIColors>()?.SetPalette(m_palettes[index]);
			}
		}
	}
}