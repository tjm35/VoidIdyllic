using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;

namespace Moonshot.Player
{
	public class FsmSetVehicleType : MonoBehaviour
	{
		public PlayMakerFSM m_fsm;

		void Start()
		{
			m_vehicleTypeVariable = m_fsm.FsmVariables.FindFsmEnum("VehicleType");
		}

		void Update()
		{
			Transform t = transform;
			while (t != null && t.GetComponent<PlayerVehicle>() == null)
			{
				t = t.parent;
			}
			var vehicle = t.GetComponent<PlayerVehicle>();
			if (vehicle)
			{
				m_vehicleTypeVariable.Value = vehicle.m_type;
			}
		}

		private FsmEnum m_vehicleTypeVariable;
	}
}