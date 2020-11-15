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
			Alarm,
		}

		public Color Primary = Color.white;
		public Color Background = Color.black;
		public Color ReversePrimary = Color.black;
		public Color Borders = Color.white;
		public Color Alarm = Color.red;

		public Color Get(ColorType i_type)
		{
			switch (i_type)
			{
				case ColorType.Primary:
					return Primary;
				case ColorType.Background:
					return Background;
				case ColorType.ReversePrimary:
					return ReversePrimary;
				case ColorType.Borders:
					return Borders;
				case ColorType.Alarm:
					return Alarm;
			}
			return new Color(1.0f, 0.6f, 0.8f);
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