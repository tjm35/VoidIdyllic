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
		public int m_triggerDelayFrames = 1;

		private void OnEnable()
		{
			//Debug.Log("EmergencyToggleSelector activated");
			if (m_lastActive != null)
			{
				//Debug.Log("Marking restore needed, restoring " + m_lastActive.name);
				m_restoreNeeded = true;
			}
		}

		private void Update()
		{
			Debug.Assert(m_targetGroup);
			Debug.Assert(m_targetGroup.isActiveAndEnabled, "EmergencyToggleSelector: Target group is not active and enabled.");

			if (m_restoreNeeded && m_lastActive != null && m_lastActive.isActiveAndEnabled)
			{
				//Debug.Log("Using restore needed, restoring " + m_lastActive.name);
				m_lastActive.isOn = true;
				m_restoreNeeded = false;
			}

			if (!m_targetGroup.AnyTogglesOn())
			{
				m_triggerDelayCount++;
				if (m_triggerDelayCount > m_triggerDelayFrames)
				{
					if (m_lastActive != null && m_lastActive.isActiveAndEnabled)
					{
						m_lastActive.isOn = true;
					}
					else if (m_preferredToggle.isActiveAndEnabled && m_preferredToggle.IsInteractable())
					{
						//Debug.Log($"Activating preferred toggle (lastActive is {(m_lastActive != null ? m_lastActive.name : "null")}, active is {m_lastActive?.isActiveAndEnabled})");

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
			}
			else
			{
				m_triggerDelayCount = 0;
			}

			if (!HasSelection())
			{
				var current = GetActiveToggle();
				if (current)
				{
					current.Select();
				}
			}

			var activeToggle = GetActiveToggle();
			if (activeToggle != null && activeToggle != m_lastActive)
			{
				//Debug.Log("Setting lastactive to " + activeToggle);
				m_lastActive = activeToggle;
			}
		}

		private Toggle GetActiveToggle()
		{
			return m_targetGroup.ActiveToggles().Where(t => t.isOn).FirstOrDefault();
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

		private Toggle m_lastActive;
		private bool m_restoreNeeded;
		private int m_triggerDelayCount = 0;
	}
}