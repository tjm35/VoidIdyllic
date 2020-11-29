using Moonshot.Photos;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class TransitControlConsole : MonoBehaviour
	{
		public TransitCore m_core;
		public int m_destID = 0;

		public void OnPhotoTaken(PointOfInterest i_poi)
		{
			m_core.Activate(m_destID);
		}
	}
}