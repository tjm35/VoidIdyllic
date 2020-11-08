using System.Collections;
using System.Collections.Generic;
using Moonshot.Player;
using UnityEngine;

namespace Moonshot.Ship
{
	[RequireComponent(typeof(PlayerVehicle))]
	public class ShipTakeoff : MonoBehaviour
	{
		public GameObject InFlightPrefab;

		public bool CanTakeoff()
		{
			// TODO: Check there's enough room.
			return true;
		}

		[ContextMenu("Debug Perform Takeoff")]
		public void PerformTakeoff()
		{
			var go = Instantiate(InFlightPrefab, transform.parent);
			go.transform.localPosition = transform.localPosition;
			go.transform.localRotation = transform.localRotation;

			// TODO: Put ship into a takeoff mode so it doesn't immediately land again.
			// For now we just teleport it up 5m
			go.transform.position += 10.0f * transform.up;

			var playerSoul = GetComponent<PlayerVehicle>().PlayerSoul;
			go.GetComponent<PlayerVehicle>().PlayerSoul = playerSoul;

			playerSoul.transform.SetParent(go.GetComponent<PlayerVehicle>().SoulHook, false);

			Destroy(gameObject);
		}
	}
}