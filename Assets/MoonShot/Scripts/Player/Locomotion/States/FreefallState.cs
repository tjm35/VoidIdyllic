using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality.StateMachine;

namespace Moonshot.Player.Locomotion
{
	public class FreefallState : State<Locomotion>
	{
		public FreefallState(float i_currentFallTime = 0.0f)
		{
			m_fallTime = i_currentFallTime;
		}

		public override void UpdateState(StateMachine<Locomotion> i_machine, Locomotion i_locomotion)
		{
			if (i_locomotion.CharacterController.IsGrounded)
			{
				float verticalImpactVelocity = -Vector3.Dot(i_locomotion.CharacterController.Velocity, i_locomotion.CharacterController.UpWS);
				float totalImpactVelocity = i_locomotion.CharacterController.Velocity.magnitude;
				float impactVelocity = Mathf.Lerp(totalImpactVelocity, verticalImpactVelocity, 0.8f);
				i_locomotion.LocomotionAudio.DoLanding(i_locomotion.CharacterController.LastGroundMaterial, impactVelocity);
				i_machine.SwitchToState(new LocomotingState(), i_locomotion);
			}
			if (i_locomotion.LocomotionControls.Jump == true)
			{
				i_machine.SwitchToState(new JetpackState(false, i_locomotion), i_locomotion);
			}
		}

		public override void FixedUpdate(Locomotion i_locomotion)
		{
			m_fallTime += Time.fixedDeltaTime;

			Vector3 velWS = i_locomotion.GetBaseVelocityIntentionWS();
			i_locomotion.ApplyStickMovement(ref velWS);
			i_locomotion.ApplyFalling(ref velWS, m_fallTime);
			i_locomotion.ConfirmVelocity(velWS);
			i_locomotion.UpdatePlayerLook();
		}

		private float m_fallTime = 0.0f;
	}
}