using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Moonshot.UI
{
	[RequireComponent(typeof(Toggle))]
	public class MenuToggleNavigationControl : MonoBehaviour
	{
		private void Start()
		{
			m_toggle = GetComponent<Toggle>();
		}
		private void Update()
		{
			var nav = m_toggle.navigation;

			if (m_toggle.isOn)
			{
				// We're active, so let the user navigate to us.
				nav.mode = Navigation.Mode.Automatic;
			}
			else
			{
				if (EventSystem.current.currentSelectedGameObject && m_toggle.group.ActiveToggles().Contains(EventSystem.current.currentSelectedGameObject.GetComponent<Toggle>()))
				{
					// We're off but one of the other toggles in our group is selected, so let the user navigate to us from it.
					nav.mode = Navigation.Mode.Automatic;
				}
				else
				{
					// We're off and nothing in our toggle group is selected, so disable navigation to us.
					nav.mode = Navigation.Mode.None;
				}
			}

			m_toggle.navigation = nav;
		}

		private Toggle m_toggle;
	}
}