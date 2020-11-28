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
		public bool m_showFPS = true;

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
			string fps = "";

			m_timeHistory.Add(Time.unscaledDeltaTime);
			if (m_timeHistory.Count > 50)
			{
				m_timeHistory.RemoveAt(0);
			}

			if (m_showFPS)
			{
				float totalTime = 0.0f;
				for (int i = 0; i < m_timeHistory.Count; ++i)
				{
					totalTime += m_timeHistory[i];
				}
				fps = "FPS: " + (1.0f / Time.unscaledDeltaTime).ToString() + "\nSmoothedFPS: " + (m_timeHistory.Count / totalTime) + "\n";
			}
			m_text.text = fps + m_stringBuilder.ToString();
			m_stringBuilder.Clear();
		}

		private static QuickDebug s_instance;
		private TMP_Text m_text;
		private StringBuilder m_stringBuilder = new StringBuilder();
		private List<float> m_timeHistory = new List<float>();
	}
}