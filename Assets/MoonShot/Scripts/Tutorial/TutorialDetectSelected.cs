using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Moonshot.Tutorial
{
	[RequireComponent(typeof(Selectable))]
	public class TutorialDetectSelected : MonoBehaviour
	{
		public string m_variableName;

		// Update is called once per frame
		void Update()
		{
			TutorialHelper.SetBool(m_variableName, EventSystem.current.currentSelectedGameObject == gameObject);
		}
	}
}