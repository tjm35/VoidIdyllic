using System.Collections;
using System.Collections.Generic;
using Moonshot.Ship;
using UnityEngine;

namespace Moonshot.UI
{
	public class ShipTakeoffUITrigger : MonoBehaviour
	{
		public void PerformTakeoff()
		{
			var takeoff = transform.GetComponentInAncestors<ShipTakeoff>();
			if (takeoff)
			{
				takeoff.PerformTakeoff();
			}
		}
	}
}