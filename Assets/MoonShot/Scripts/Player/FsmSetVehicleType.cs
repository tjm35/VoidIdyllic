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
			m_vehicleTypeVariable.Value = PlayerVehicle.Current.m_type;
		}

		private FsmEnum m_vehicleTypeVariable;
	}
}