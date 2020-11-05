using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.World;

namespace Moonshot.Ship
{
	[RequireComponent(typeof(ShipControls))]
	[RequireComponent(typeof(ShipController))]
	public class ShipMovement : MonoBehaviour
	{
		public GameObject GravityProviderObject;

		public float m_baseAccel = 5.0f;

		public float m_maxAccelScaleMultiplier = 40.0f;
		public float m_maxAccelScaleMultiplierDistance = 2000.0f;

		public float m_pitchSpeed = 90.0f;
		public float m_yawSpeed = 90.0f;
		public float m_rollSpeed = 45.0f;

		public float m_decelSpeedHalfLife = 2.0f;
		public float m_minDecel = 1.0f;

		void Start()
		{
			m_controls = GetComponent<ShipControls>();
			m_controller = GetComponent<ShipController>();
			if (GravityProviderObject)
			{
				m_gravityProvider = GravityProviderObject.GetComponent<IGravityProvider>();
			}
			else
			{
				m_gravityProvider = GlobalGravityProvider.Instance;
			}
		}
		// Update is called once per frame
		void Update()
		{
			UpdateLook();
			UpdateMove();
		}

		void UpdateLook()
		{
			Vector3 lookLS = m_controls.Look;
			Quaternion turnAmount = Quaternion.Euler(-lookLS.x * m_pitchSpeed * Time.deltaTime, lookLS.y * m_yawSpeed * Time.deltaTime, -lookLS.z * m_rollSpeed * Time.deltaTime);
			transform.localRotation = transform.localRotation * turnAmount;
		}

		void UpdateMove()
		{
			Vector3 moveLS = m_controls.Move;

			Vector3 accel = GetAccel();
			Vector3 accelDelta = accel * Time.deltaTime;

			// Split current velocity into intended and unintended components.
			Vector3 oldVelocity = m_controller.WorldVelocity;
			Vector3 oldVelocityIntended = Vector3.zero;
			if (!Mathf.Approximately(accel.sqrMagnitude, 0.0f) && !Mathf.Approximately(oldVelocity.sqrMagnitude, 0.0f))
			{
				float intendedDirectionSpeed = Mathf.Max(Vector3.Dot(oldVelocity, accel.normalized), 0.0f);
				oldVelocityIntended = intendedDirectionSpeed * accel.normalized;
			}
			Vector3 oldVelocityUnintended = oldVelocity - oldVelocityIntended;

			float unintendedSpeed = oldVelocityUnintended.magnitude;
			if (unintendedSpeed < m_minDecel * Time.deltaTime)
			{
				m_controller.WorldVelocity = oldVelocityIntended + accelDelta;
			}
			else
			{
				float k = Mathf.Log(2) / m_decelSpeedHalfLife;
				float minDecelProp = Mathf.Clamp01(m_minDecel * Time.deltaTime / unintendedSpeed);
				float decelProp = Mathf.Clamp(k * Time.deltaTime, minDecelProp, 1.0f);
				Vector3 newVelocityUnintended = (1.0f - decelProp) * oldVelocityUnintended;

				m_controller.WorldVelocity = oldVelocityIntended + newVelocityUnintended + accelDelta;
			}
		}

		private Vector3 GetAccel()
		{
			Vector3 moveLS = m_controls.Move;

			// Button pressed, so accelerate.
			//Debug.Log($"Gravity prominence = {m_gravityProvider.GetGravityProminence(transform.position)}");
			Vector3 globalPos = LocalFrame.TransformPointToGlobal(LocalFrame.Get(transform), transform.position);
			float approxBodyDistance = Mathf.Sqrt(1.0f / m_gravityProvider.GetGravityProminence(globalPos));
			//Debug.Log($"Approx body distance: {approxBodyDistance}");
			float accelMultiplier = Mathf.Clamp(m_maxAccelScaleMultiplier * approxBodyDistance / m_maxAccelScaleMultiplierDistance, 1.0f, m_maxAccelScaleMultiplier);

			// For now, just naive
			Vector3 moveWS = transform.TransformVector(moveLS);

			return moveWS * m_baseAccel * accelMultiplier;
		}

		private IGravityProvider m_gravityProvider;
		private ShipControls m_controls;
		private ShipController m_controller;
	}
}