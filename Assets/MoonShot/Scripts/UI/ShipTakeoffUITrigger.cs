using System.Collections;
using System.Collections.Generic;
using Moonshot.Ship;
using PlayMaker;
using UnityEngine;

namespace Moonshot.UI
{
	public class ShipTakeoffUITrigger : MonoBehaviour
	{
		public PlayMakerFSM m_menuFSM;

		public void PerformTakeoff()
		{
			var takeoff = transform.GetComponentInAncestors<ShipTakeoff>();
			if (takeoff)
			{
				takeoff.PerformTakeoff();
			}
			if (m_menuFSM)
			{
				m_menuFSM.SendEvent("ExitMenu");
			}
		}
	}
}