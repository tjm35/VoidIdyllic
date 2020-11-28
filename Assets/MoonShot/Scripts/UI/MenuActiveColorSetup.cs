using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Moonshot.UI
{
	public class MenuActiveColorSetup : MonoBehaviour, UIColors.Settable
	{
		public UIColors.ColorType m_normalColorType = UIColors.ColorType.Borders;

		// Start is called before the first frame update
		void Start()
		{
			m_controllingToggle = transform.GetComponentInAncestors<Toggle>();
			m_graphic = GetComponent<Graphic>();
			var uicolors = transform.GetComponentInAncestors<UIColors>();
			if (uicolors)
			{
				SetupColors(uicolors);
			}
		}

		public void SetupColors(UIColors i_colors)
		{
			m_normalColor = i_colors.Get(m_normalColorType);
			m_selectedColor = i_colors.Get(UIColors.ColorType.Selected);
		}

		// Update is called once per frame
		void Update()
		{
			if (EventSystem.current.currentSelectedGameObject == m_controllingToggle.gameObject)
			{
				m_graphic.color = m_selectedColor;
			}
			else
			{
				m_graphic.color = m_normalColor;
			}
		}

		private Graphic m_graphic;
		private Toggle m_controllingToggle;
		private Color m_normalColor;
		private Color m_selectedColor;
	}
}