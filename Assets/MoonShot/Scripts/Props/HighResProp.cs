using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.Photos;

namespace Moonshot.Props
{
	public class HighResProp : MonoBehaviour
	{
		public OrreryProp OrreryProp
		{
			get { return m_orreryProp; }
			set
			{
				Debug.Assert(m_orreryProp == null);
				m_orreryProp = value;
				if (enabled)
				{
					m_orreryProp.EnableHighRes(this);
				}
				if (GetComponent<POIReference>())
				{
					GetComponent<POIReference>().m_referencedPOI = m_orreryProp.m_pointOfInterest;
				}
				SendMessage("OnOrreryPropSet", m_orreryProp, SendMessageOptions.DontRequireReceiver);
			}
		}

		private void OnEnable()
		{
			if (m_orreryProp)
			{
				m_orreryProp.EnableHighRes(this);
			}
		}

		private void OnDisable()
		{
			if (m_orreryProp)
			{
				m_orreryProp.DisableHighRes(this);
			}
		}

		private void OnDestroy()
		{
			if (m_orreryProp && enabled)
			{
				m_orreryProp.DisableHighRes(this);
			}
		}

		private OrreryProp m_orreryProp = null;
	}
}