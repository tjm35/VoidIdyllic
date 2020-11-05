using System.Collections;
using System.Collections.Generic;
using Moonshot.Player;
using UnityEngine;

namespace Moonshot.Ship
{
	[RequireComponent(typeof(PlayerVehicle))]
	public class ShipLander : MonoBehaviour
	{
		public GameObject OnFootPrefab;

		public void OnShipCollided(Object i_collider)
		{
			// TODO - Check if safe to land
			PerformLanding();
		}

		public void PerformLanding()
		{
			var go = Instantiate(OnFootPrefab, transform.parent);
			go.transform.localPosition = transform.localPosition;
			go.transform.localRotation = transform.localRotation;

			var playerSoul = GetComponent<PlayerVehicle>().PlayerSoul;
			go.GetComponent<PlayerVehicle>().PlayerSoul = playerSoul;

			playerSoul.transform.SetParent(go.GetComponent<PlayerVehicle>().SoulHook, false);

			Destroy(gameObject);
		}
	}
}