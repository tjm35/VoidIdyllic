using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

namespace Moonshot.UI
{
	[RequireComponent(typeof(TMP_Text))]
	public class QuickDebug : MonoBehaviour
	{
		public static void Print(string i_text)
		{
			if (s_instance)
			{
				s_instance.m_stringBuilder.AppendLine(i_text);
			}
		}

		private void Start()
		{
			m_text = GetComponent<TMP_Text>();
			s_instance = this;
		}

		private void OnDestroy()
		{
			s_instance = null;
		}

		private void LateUpdate()
		{
			m_text.text = m_stringBuilder.ToString();
			m_stringBuilder.Clear();
		}

		private static QuickDebug s_instance;
		private TMP_Text m_text;
		private StringBuilder m_stringBuilder = new StringBuilder();
	}
}