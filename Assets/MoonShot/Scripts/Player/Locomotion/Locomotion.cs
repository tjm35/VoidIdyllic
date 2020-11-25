using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality.StateMachine;
using Moonshot.World;

namespace Moonshot.Player.Locomotion
{
	[RequireComponent(typeof(LocomotionControls))]
	[RequireComponent(typeof(CharacterController))]
	public class Locomotion : MonoBehaviour
	{
		public LocomotionControls LocomotionControls { get; private set; }
		public CharacterController CharacterController { get; private set; }

		public Transform m_cameraBone;
		public float m_speed = 3.0f;
		public float m_fallSpeed = 3.0f;
		public float m_fallAccelTime = 3.0f;
		public AnimationCurve m_fallSpeedCurve;
		public float m_xRotSpeed = 120.0f;
		public float m_yRotSpeed = 270.0f;
		public float m_dragTurnMultiplier = 1.0f;
		public float m_jumpBoostTime = 3.0f;
		public float m_jumpBoostSpeed = 3.0f;
		public AnimationCurve m_jumpBoostCurve;
		public float m_maxGravityEffect = 1.0f;
		public float m_minGravityEffect = 0.25f;

		public string debug_State = "Unset";

		void Start()
		{
			LocomotionControls = GetComponent<LocomotionControls>();
			CharacterController = GetComponent<CharacterController>();

			m_stateMachine = new StateMachine<Locomotion>(new InitialState(), this);
		}

		private void OnEnable()
		{
			Tutorial.TutorialHelper.SetBool("IsOnPlanet", true);
		}

		private void OnDisable()
		{
			Tutorial.TutorialHelper.SetBool("IsOnPlanet", false);
		}

		void Update()
		{
			m_stateMachine.Update(this);
			debug_State = m_stateMachine.CurrentState.ToString();
		}

		void FixedUpdate()
		{
			m_stateMachine.FixedUpdate(this);
			debug_State = m_stateMachine.CurrentState.ToString();
		}

		public Vector3 GetBaseVelocityIntentionWS()
		{
			return Vector3.zero;
		}

		public void ApplyStickMovement(ref Vector3 io_velocityWS)
		{
			Vector3 moveInputVector = LocomotionControls.MoveInstructionWS;
			Vector3 flattenedMoveInput = moveInputVector - CharacterController.UpWS * Vector3.Dot(CharacterController.UpWS, moveInputVector);
			io_velocityWS += flattenedMoveInput * m_speed;
		}

		public void ApplyFalling(ref Vector3 io_velocityWS, float i_fallTime)
		{
			//UI.QuickDebug.Print($"Gravity magnitude: {GlobalGravityProvider.Instance.GetGravity(LocalFrame.GetGlobalPosition(transform)).magnitude}");

			io_velocityWS -= CharacterController.UpWS * GetFallSpeed(i_fallTime);
		}

		public void ApplyJumpBoost(ref Vector3 io_velocityWS, float i_timeProp)
		{
			io_velocityWS += CharacterController.UpWS * GetJumpSpeed(i_timeProp);
		}

		public void ConfirmVelocity(Vector3 i_velocityWS)
		{
			CharacterController.MoveWorldVelocity(i_velocityWS);
		}

		public void UpdatePlayerLook()
		{
			Vector2 lookTurn = LocomotionControls.Look;

			if (m_cameraBone)
			{
				float lookUp = m_cameraBone.localRotation.eulerAngles.x;
				while (lookUp > 180.0f)
				{
					lookUp -= 360.0f;
				}
				while (lookUp < -180.0f)
				{
					lookUp += 360.0f;
				}
				lookUp += -lookTurn.y * m_xRotSpeed * Time.fixedDeltaTime;
				lookUp = Mathf.Clamp(lookUp, -90.0f, 90.0f);
				m_cameraBone.localRotation = Quaternion.Euler(lookUp, 0.0f, 0.0f);
			}

			transform.Rotate(CharacterController.UpWS, lookTurn.x * m_yRotSpeed * Time.fixedDeltaTime, Space.World);
		}

		private float GetJumpSpeed(float i_timeProp)
		{
			return m_jumpBoostSpeed * m_jumpBoostCurve.Evaluate(i_timeProp) / GetGravityEffect();
		}

		private float GetFallSpeed(float i_fallTime)
		{
			return m_fallSpeed * m_fallSpeedCurve.Evaluate(Mathf.Clamp01(i_fallTime / m_fallAccelTime)) * GetGravityEffect();
		}

		private float GetGravityEffect()
		{
			return Mathf.Clamp(GlobalGravityProvider.Instance.GetGravity(LocalFrame.GetGlobalPosition(transform)).magnitude, m_minGravityEffect, m_maxGravityEffect);
		}

		private StateMachine<Locomotion> m_stateMachine;
	}
}