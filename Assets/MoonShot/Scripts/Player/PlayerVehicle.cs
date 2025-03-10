﻿using Moonshot.World;
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

		public void AttachPlayerMaintainingPosition(GameObject i_playerSoul)
		{ 
			PlayerSoul = i_playerSoul;

			transform.rotation = i_playerSoul.transform.rotation * Quaternion.Inverse(Quaternion.Inverse(transform.rotation) * SoulHook.rotation);
			transform.position = i_playerSoul.transform.position + (transform.position - SoulHook.position);

			i_playerSoul.transform.SetParent(SoulHook, false);
		}

		public void TeleportToGlobal(Vector3 i_globalPos, Quaternion i_globalRot)
		{
			LocalFrame lf = LocalFrame.Get(transform);
			Vector3 localPos = LocalFrame.TransformPointToLocal(lf, i_globalPos);
			Quaternion localRot = LocalFrame.TransformRotationToLocal(lf, i_globalRot);

			transform.position = localPos;
			transform.rotation = localRot;
		}

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