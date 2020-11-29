using Moonshot.Player;
using Moonshot.World;
using OVR;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class TransitCore : MonoBehaviour
	{
		public SoundFXRef m_ruinFX;
		public GameObject[] m_chunkObjects;
		public string[] m_targetNames;
		public float m_activeTime = 3.0f;
		public float m_audioFadeTime = 0.1f;
		public Transform m_pad;
		public float m_padHalfHeight = 3.0f;
		public float m_padRadius = 3.0f;

		public void Activate(int i_dest)
		{
			for (int i = 0; i < m_chunkObjects.Length; ++i)
			{
				m_chunkObjects[i].SetActive(i == i_dest);
			}
			if (m_fxID != -1)
			{
				AudioManager.FadeOutSound(m_fxID, m_audioFadeTime);
			}
			m_fxID = m_ruinFX.PlaySound();
			m_activeTimer = 0.0f;
			m_activeObject = i_dest;
			m_active = true;
		}

		public bool IsActive(int i_dest)
		{
			return m_activeObject == i_dest;
		}

		private void Update()
		{
			if (m_active)
			{
				m_activeTimer += Time.deltaTime;
				if (m_activeTimer > m_activeTime)
				{
					for (int i = 0; i < m_chunkObjects.Length; ++i)
					{
						m_chunkObjects[i].SetActive(false);
					}
					if (m_fxID != -1)
					{
						AudioManager.FadeOutSound(m_fxID, m_audioFadeTime);
					}
					m_active = false;
					int dest = m_activeObject;
					m_activeObject = -1;
					PerformTeleport(dest);
				}
			}
		}

		private void PerformTeleport(int i_destID)
		{
			LocalFrame lf = LocalFrame.Get(m_pad);
			Vector3 playerGlobalPos = LocalFrame.GetGlobalPosition(PlayerVehicle.Current.transform);
			Vector3 playerLocalFramePos = LocalFrame.TransformPointToLocal(lf, playerGlobalPos);
			Vector3 playerPadLocalPos = m_pad.InverseTransformPoint(playerLocalFramePos);

			if 
			(
				Mathf.Abs(playerPadLocalPos.y) < m_padHalfHeight &&
				new Vector2(playerPadLocalPos.x, playerPadLocalPos.z).magnitude < m_padRadius
			)
			{
				//Debug.Log("Player in teleport zone.");
				GameObject target = GameObject.Find(m_targetNames[i_destID]);

				LocalFrame playerFrame = LocalFrame.Get(PlayerVehicle.Current.transform);
				Quaternion playerGlobalRot = LocalFrame.TransformRotationToGlobal(playerFrame, PlayerVehicle.Current.transform.rotation);
				Quaternion playerLocalFrameRot = LocalFrame.TransformRotationToLocal(lf, playerGlobalRot);
				Quaternion playerPadLocalRotation = Quaternion.Inverse(m_pad.rotation) * playerLocalFrameRot;

				Vector3 playerDestFramePos = target.transform.TransformPoint(playerPadLocalPos);
				Quaternion playerDestFrameRot = target.transform.rotation * playerPadLocalRotation;

				LocalFrame destFrame = LocalFrame.Get(target.transform);
				Vector3 playerDestGlobalPos = LocalFrame.TransformPointToGlobal(destFrame, playerDestFramePos);
				Quaternion playerDestGlobalRot = LocalFrame.TransformRotationToGlobal(destFrame, playerDestFrameRot);
				PlayerVehicle.Current.TeleportToGlobal(playerDestGlobalPos, playerDestGlobalRot);
			}
		}

		private bool m_active = false;
		private int m_fxID = -1;
		private float m_activeTimer = 0.0f;
		private int m_activeObject = -1;
	}
}