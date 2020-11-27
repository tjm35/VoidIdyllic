using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Moonshot.Photos
{
	public class GoalUIEntry : MonoBehaviour
	{
		public TMP_Text m_text;
		public Toggle m_toggle;
		public RectTransform m_checkmarkMask;
		public GameObject m_firstCompleteHighlight;
		public float m_checkmarkMaskWidth = 120.0f;
		public float m_completeAnimationDuration = 1.5f;
		public float m_completeAnimationDelay = 0.5f;

		public Goal Goal
		{
			get { return m_goal; }
			set
			{
				m_goal = value;
				m_text.text = m_goal.GetDescription();
			}
		}

		public bool FirstTimeComplete
		{
			set
			{
				m_firstTimeComplete = value;
				m_completeAnimationPlayTime = 0.0f;
			}
		}

		private void Start()
		{
			m_cloudUI = transform.GetComponentInAncestors<CloudUploadUI>();
		}

		private void Update()
		{
			m_completeAnimationPlayTime += Time.unscaledDeltaTime;
			if (m_goal)
			{
				if (m_cloudUI)
				{
					m_toggle.isOn = m_cloudUI.IsGoalMetBySelectedPhotos(m_goal);
				}
				else
				{
					m_toggle.isOn = PhotoSystem.Instance.IsGoalComplete(m_goal);
				}
			}
			if (m_checkmarkMask)
			{
				float width = m_checkmarkMaskWidth;
				if (m_firstTimeComplete)
				{
					width *= Mathf.Clamp01((m_completeAnimationPlayTime - m_completeAnimationDelay) / m_completeAnimationDuration);
				}
				m_checkmarkMask.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width);
			}
			if (m_firstCompleteHighlight)
			{
				m_firstCompleteHighlight.SetActive(m_firstTimeComplete);
			}
		}

		private Goal m_goal;
		private CloudUploadUI m_cloudUI;
		private bool m_firstTimeComplete = false;
		private float m_completeAnimationPlayTime = 0.0f;
	}
}