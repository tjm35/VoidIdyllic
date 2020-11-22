using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.Photos;
using Moonshot.World;

namespace Moonshot.Props
{
	public class OrreryProp : MonoBehaviour
	{
		public GameObject m_highResPrefab;
		public PointOfInterest m_pointOfInterest;
		public GameObject m_moveToHighResObject;

		public void EnableHighRes(HighResProp i_highResObject)
		{
			m_highResProps.Add(i_highResObject);
			gameObject.SetActive(false);
		}

		public void DisableHighRes(HighResProp i_highResObject)
		{
			Debug.Assert(m_highResProps.Contains(i_highResObject));
			m_highResProps.Remove(i_highResObject);
			if (m_highResProps.Count == 0)
			{
				gameObject.SetActive(true);
			}
			if (m_moveToHighResObject)
			{
				m_moveToHighResObject.transform.SetParent(transform, false);
				m_moveToHighResObject.layer = gameObject.layer;
			}
		}

		public GameObject CreateHighResProp(LocalFrame i_frame)
		{
			var go = Instantiate(m_highResPrefab, i_frame.transform, false);
			go.transform.position = i_frame.TransformPointToLocal(transform.position);
			go.transform.rotation = i_frame.TransformRotationToLocal(transform.rotation);
			go.transform.localScale = transform.localScale;
			Debug.Assert(go.GetComponent<HighResProp>());
			go.GetComponent<HighResProp>().OrreryProp = this;
			if (m_moveToHighResObject)
			{
				m_moveToHighResObject.transform.SetParent(go.transform, false);
				m_moveToHighResObject.layer = go.layer;
			}
			return go;
		}

		private void Start()
		{
			if (m_pointOfInterest == null)
			{
				m_pointOfInterest = GetComponent<PointOfInterest>();
			}
		}

		private HashSet<HighResProp> m_highResProps = new HashSet<HighResProp>();
	}
}