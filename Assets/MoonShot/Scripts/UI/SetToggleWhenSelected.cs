using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Moonshot.UI
{
	[RequireComponent(typeof(Toggle))]
	public class SetToggleWhenSelected : MonoBehaviour, ISelectHandler
	{
		// Start is called before the first frame update
		void Awake()
		{
			m_toggle = GetComponent<Toggle>();
		}

		public void OnSelect(BaseEventData eventData)
		{
			m_toggle.isOn = true;
		}

		private Toggle m_toggle;
	}
}