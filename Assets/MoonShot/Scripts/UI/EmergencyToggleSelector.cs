using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Moonshot.UI
{
	public class EmergencyToggleSelector : MonoBehaviour
	{
		[Required]
		public ToggleGroup m_targetGroup;
		public Toggle m_preferredToggle;
		
		private void Update()
		{
			Debug.Assert(m_targetGroup);
			Debug.Assert(m_targetGroup.isActiveAndEnabled, "EmergencyToggleSelector: Target group is not active and enabled.");

			if (!m_targetGroup.AnyTogglesOn())
			{
				if (m_preferredToggle.isActiveAndEnabled && m_preferredToggle.IsInteractable())
				{
					m_preferredToggle.isOn = true;
				}
				else
				{
					Toggle first = m_targetGroup.GetFirstActiveToggle();
					if (first)
					{
						first.isOn = true;
					}
				}
			}

			if (!HasSelection())
			{
				var current = m_targetGroup.ActiveToggles().Where(t => t.isOn).FirstOrDefault();
				if (current)
				{
					current.Select();
				}
			}
		}

		private bool HasSelection()
		{
			if (EventSystem.current.currentSelectedGameObject != null)
			{
				var selectable = EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>();

				if (selectable && selectable.isActiveAndEnabled)
				{
					// Hopefully EventSystem already takes care of this.
					Debug.Assert(EventSystem.current.currentSelectedGameObject.GetComponent<Selectable>().IsInteractable());
					return true;
				}
			}
			return false;
		}
	}
}