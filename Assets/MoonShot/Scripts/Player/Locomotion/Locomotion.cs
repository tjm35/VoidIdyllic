using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality.StateMachine;

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
		public float m_xRotSpeed = 120.0f;
		public float m_yRotSpeed = 270.0f;
		public float m_dragTurnMultiplier = 1.0f;
		public float m_jumpBoostTime = 3.0f;
		public float m_jumpBoostSpeed = 3.0f;

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
		}

		void FixedUpdate()
		{
			m_stateMachine.FixedUpdate(this);
		}

		public void UpdateStickMovement(bool i_falling, bool i_jumpBoost)
		{
			Vector3 moveInputVector = LocomotionControls.MoveInstructionWS;
			Vector3 flattenedMoveInput = moveInputVector - CharacterController.UpWS * Vector3.Dot(CharacterController.UpWS, moveInputVector);
			Vector3 moveVelocity = flattenedMoveInput * m_speed;

			if (i_falling)
			{
				moveVelocity -= CharacterController.UpWS * m_fallSpeed;
			}
			if (i_jumpBoost)
			{
				moveVelocity += CharacterController.UpWS * m_jumpBoostSpeed;
			}

			CharacterController.MoveWorldVelocity(moveVelocity);

			UpdatePlayerLook();

			debug_State = m_stateMachine.CurrentState.ToString();
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

		private StateMachine<Locomotion> m_stateMachine;
	}
}