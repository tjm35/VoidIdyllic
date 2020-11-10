using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.World;

namespace Moonshot.Props
{
	public class OrreryProp : MonoBehaviour
	{
		public GameObject m_highResPrefab;

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
				gameObject.SetActive(false);
			}
		}

		public GameObject CreateHighResProp(LocalFrame i_frame)
		{
			var go = Instantiate(m_highResPrefab, i_frame.transform, false);
			go.transform.position = i_frame.TransformPointToLocal(transform.position);
			go.transform.rotation = i_frame.TransformRotationToLocal(transform.rotation);
			Debug.Assert(go.GetComponent<HighResProp>());
			go.GetComponent<HighResProp>().OrreryProp = this;
			return go;
		}

		private HashSet<HighResProp> m_highResProps = new HashSet<HighResProp>();
	}
}