using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Player
{
	public class PlayerVehicle : MonoBehaviour
	{
		public enum VehicleType
		{
			OnFoot,
			InFlight,
		}

		public GameObject PlayerSoul;
		public Transform SoulHook;
		public VehicleType m_type;
	}
}