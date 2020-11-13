using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Player
{
	public class PlayerVehicle : MonoBehaviour
	{
		public static PlayerVehicle Current { get; private set; }

		public enum VehicleType
		{
			OnFoot,
			InFlight,
		}

		public GameObject PlayerSoul
		{
			get { return m_playerSoul; }
			set { m_playerSoul = value; Current = this; }
		}
		public Transform SoulHook;
		public VehicleType m_type;

		private void Awake()
		{
			if (m_playerSoul)
			{
				Current = this;
			}
		}

		[SerializeField]
		private GameObject m_playerSoul;
	}
}