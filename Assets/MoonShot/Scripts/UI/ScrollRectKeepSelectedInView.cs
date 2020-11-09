using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollRectKeepSelectedInView : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
		m_scrollRect = GetComponent<ScrollRect>();
    }

    // Update is called once per frame
    void Update()
    {
		var selected = EventSystem.current?.currentSelectedGameObject;
		if (selected && selected != m_lastSelected && selected.transform.IsDescendentOf(m_scrollRect.transform) && selected.GetComponent<RectTransform>())
		{
			var rt = selected.GetComponent<RectTransform>();
			if (m_scrollRect.vertical)
			{
				m_scrollRect.verticalNormalizedPosition = EnsureContentLocalPointInViewVertical(rt.localPosition.y + rt.rect.yMin);
				m_scrollRect.verticalNormalizedPosition = EnsureContentLocalPointInViewVertical(rt.localPosition.y + rt.rect.yMax);
			}
			if (m_scrollRect.horizontal)
			{
				m_scrollRect.horizontalNormalizedPosition = EnsureContentLocalPointInViewHorizontal(rt.localPosition.x + rt.rect.xMin);
				m_scrollRect.horizontalNormalizedPosition = EnsureContentLocalPointInViewHorizontal(rt.localPosition.x + rt.rect.xMax);
			}
		}
		m_lastSelected = selected;
    }

	private float EnsureContentLocalPointInViewVertical(float i_pointY)
	{
		float currentProp = m_scrollRect.verticalNormalizedPosition;
		float viewportHeight = m_scrollRect.viewport.rect.height;
		float contentHeight = m_scrollRect.content.rect.height;
		float scrollHeight = contentHeight - viewportHeight;
		if (contentHeight > 0.0f && scrollHeight > 0.0f)
		{
			float viewportHeightProp = viewportHeight / scrollHeight;
			float requestedPosProp = (i_pointY - m_scrollRect.content.rect.yMin - 0.5f * viewportHeight) / scrollHeight;

			float oldProp = currentProp;
			if (requestedPosProp < currentProp - 0.5f * viewportHeightProp )
			{
				currentProp = requestedPosProp + 0.5f * viewportHeightProp;
			}
			if (requestedPosProp > currentProp + 0.5f * viewportHeightProp )
			{
				currentProp = requestedPosProp - 0.5f * viewportHeightProp;
			}
			currentProp = Mathf.Clamp01(currentProp);
			//Debug.Log($"old: {oldProp} new: {currentProp} requested: {requestedPosProp}, viewport: {viewportHeightProp}");
		}

		return currentProp;
	}

	private float EnsureContentLocalPointInViewHorizontal(float i_pointX)
	{
		float currentProp = m_scrollRect.horizontalNormalizedPosition;
		float viewportWidth = m_scrollRect.viewport.rect.width;
		float contentWidth = m_scrollRect.content.rect.width;
		float scrollWidth = contentWidth - viewportWidth;
		if (contentWidth > 0.0f && scrollWidth > 0.0f)
		{
			float viewportWidthProp = viewportWidth / scrollWidth;
			float requestedPosProp = (i_pointX - m_scrollRect.content.rect.xMin - 0.5f * viewportWidth) / scrollWidth;

			float oldProp = currentProp;
			if (requestedPosProp < currentProp - 0.5f * viewportWidthProp )
			{
				currentProp = requestedPosProp + 0.5f * viewportWidthProp;
			}
			if (requestedPosProp > currentProp + 0.5f * viewportWidthProp )
			{
				currentProp = requestedPosProp - 0.5f * viewportWidthProp;
			}
			currentProp = Mathf.Clamp01(currentProp);
		}

		return currentProp;
	}

	private ScrollRect m_scrollRect;
	private GameObject m_lastSelected;
}
