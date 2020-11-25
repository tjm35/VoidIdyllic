using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Moonshot.UI
{
	public class UIColorSetup : MonoBehaviour, UIColors.Settable
	{
		public UIColors.ColorType ColorType = UIColors.ColorType.Primary;
		public bool SetupOnStart = true;

		public void SetupColors(UIColors i_colors)
		{
			var col = i_colors.Get(ColorType);
			ApplyColor(col, i_colors);
		}

		private void Start()
		{
			if (SetupOnStart)
			{
				var uicolors = transform.GetComponentInAncestors<UIColors>();
				if (uicolors)
				{
					SetupColors(uicolors);
				}
			}
		}

		private void ApplyColor(Color i_col, UIColors i_colors)
		{
			var graphic = GetComponent<Graphic>();
			var selectable = GetComponent<Selectable>();
			if (graphic && !selectable)
			{
				graphic.color = i_col;
			}

			if (selectable)
			{
				var cb = new ColorBlock();
				cb.normalColor = i_colors.Get(UIColors.ColorType.Borders);
				cb.highlightedColor = i_colors.Get(UIColors.ColorType.Primary);
				cb.selectedColor = i_colors.Get(UIColors.ColorType.Selected);
				cb.pressedColor = i_colors.Get(UIColors.ColorType.Pressed);
				var disabledColor = i_colors.Get(UIColors.ColorType.Borders);
				disabledColor.a *= 0.25f;
				cb.disabledColor = disabledColor;
				cb.colorMultiplier = 1.0f;

				selectable.colors = cb;
			}
		}
	}
}