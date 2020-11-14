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
			ApplyColor(col);
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

		private void ApplyColor(Color i_col)
		{
			var graphic = GetComponent<Graphic>();
			if (graphic)
			{
				graphic.color = i_col;
			}
		}
	}
}