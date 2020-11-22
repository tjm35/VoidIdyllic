using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.Photos;
using Moonshot.World;

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
					m_isEnabledOnOrreryProp = true;
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
			Debug.Assert(!m_isEnabledOnOrreryProp);
			if (m_orreryProp)
			{
				m_orreryProp.EnableHighRes(this);
				m_isEnabledOnOrreryProp = true;
			}
		}

		private void OnExitLocalFrame(LocalFrame i_localFrame)
		{
			if (m_orreryProp && m_isEnabledOnOrreryProp)
			{
				m_orreryProp.DisableHighRes(this);
				m_isEnabledOnOrreryProp = false;
			}
		}

		private void OnDisable()
		{
			if (m_orreryProp && m_isEnabledOnOrreryProp)
			{
				m_orreryProp.DisableHighRes(this);
				m_isEnabledOnOrreryProp = false;
			}
		}

		private void OnDestroy()
		{
			if (m_orreryProp && m_isEnabledOnOrreryProp)
			{
				m_orreryProp.DisableHighRes(this);
				m_isEnabledOnOrreryProp = false;
			}
		}

		private OrreryProp m_orreryProp = null;
		private bool m_isEnabledOnOrreryProp = false;
	}
}