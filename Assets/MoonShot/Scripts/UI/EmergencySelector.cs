using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Moonshot.UI
{
	public class EmergencySelector : MonoBehaviour
	{
		public Selectable[] m_candidateSelectables;
		public bool m_lastResortRandom = false;
		public int m_noSelectionWaitCount = 0;
		
		private void Update()
		{
			if (HasSelection())
			{
				m_currentNoSelectionWait = 0;
			}
			else
			{
				m_currentNoSelectionWait++;
				if (m_currentNoSelectionWait > m_noSelectionWaitCount)
				{
					PerformEmergencySelect();
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

		private void PerformEmergencySelect()
		{
			foreach (var selectable in m_candidateSelectables)
			{
				if (selectable != null && selectable.isActiveAndEnabled && selectable.IsInteractable())
				{
					selectable.Select();
					Debug.Assert(HasSelection());
					return;
				}
			}

			if (m_lastResortRandom)
			{
				var selectables = transform.GetComponentsInDescendents<Selectable>().Where(s => s.isActiveAndEnabled && s.IsInteractable());
				if (selectables.Count() > 0)
				{
					selectables.ElementAt(Random.Range(0, selectables.Count())).Select();
					Debug.Assert(HasSelection());
				}
				else
				{
					Debug.LogError("EmergencySelector: Last resort selection failed! Couldn't find any selectable objects. We'll keep trying.");
				}
			}
		}

		private int m_currentNoSelectionWait = 0;
	}
}