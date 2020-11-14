using System.Collections;
using System.Collections.Generic;
using HutongGames.PlayMaker;
using Moonshot.Player;
using Moonshot.World;
using UnityEngine;

namespace Moonshot.Ship
{
	[RequireComponent(typeof(PlayerVehicle))]
	public class ShipLander : MonoBehaviour
	{
		public GameObject OnFootPrefab;
		public GameObject ShipPhantomPrefab;
		public float m_maxRightTime = 1.0f;

		public void OnShipCollided(Object i_collider)
		{
			if (!m_landing)
			{
				// TODO - Check if safe to land

				m_landing = true;
				StartCoroutine(PerformLanding());
			}
		}

		public IEnumerator PerformLanding()
		{
			LocalFrame frame = LocalFrame.Get(transform);
			if (GlobalGravityProvider.Instance.GetUp(frame.TransformPointToGlobal(transform.position), out Vector3 globalTargetUp))
			{
				var targetUp = frame.TransformVectorToLocal(globalTargetUp);
				var currentUp = transform.up;
				var upDifferenceProp = Vector3.Angle(currentUp, targetUp) / 180.0f;
				float rightTime = m_maxRightTime * upDifferenceProp;
				float rightTimeRemaining = rightTime;
				Vector3 targetForward = Vector3.Cross(transform.right, targetUp);
				Quaternion targetRot = Quaternion.LookRotation(targetForward, targetUp);

				while (rightTimeRemaining > 0.0f)
				{
					float lerpProp = Mathf.Clamp01(Time.deltaTime / rightTimeRemaining);
					transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, lerpProp);
					rightTimeRemaining -= Time.deltaTime;
					yield return null;
				}
			}

			MakeOnFootPlayer();
			MakeShipPhantom();

			Tutorial.TutorialHelper.SetBool("HasLanded");

			Destroy(gameObject);
		}

		private void MakeOnFootPlayer()
		{
			var go = Instantiate(OnFootPrefab, transform.parent);
			go.transform.localPosition = transform.localPosition;
			go.transform.localRotation = transform.localRotation;

			var playerSoul = GetComponent<PlayerVehicle>().PlayerSoul;
			go.GetComponent<PlayerVehicle>().PlayerSoul = playerSoul;

			playerSoul.transform.SetParent(go.GetComponent<PlayerVehicle>().SoulHook, false);
		}

		private void MakeShipPhantom()
		{
			var go = Instantiate(ShipPhantomPrefab, transform.parent);
			go.transform.localPosition = transform.localPosition;
			go.transform.localRotation = transform.localRotation;

			var fsm = go.GetComponent<PlayMakerFSM>();
			if (fsm)
			{
				fsm.SetState("TakeOff");
			}
		}

		private bool m_landing = false;
	}
}