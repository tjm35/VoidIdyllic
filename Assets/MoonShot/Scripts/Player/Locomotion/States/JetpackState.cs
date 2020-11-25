using System.Collections;
using System.Collections.Generic;
using Totality.StateMachine;
using UnityEngine;

namespace Moonshot.Player.Locomotion
{
	public class JetpackState : State<Locomotion>
	{
		public JetpackState(bool i_fromJump, Locomotion i_locomotion)
		{
			m_remainingBoostTime = (i_fromJump? i_locomotion.m_jumpBoostTime : 0.0f);

			if (i_fromJump)
			{
				Tutorial.TutorialHelper.SetBool("HasBoosted");
			}
		}

		public override void UpdateState(StateMachine<Locomotion> i_machine, Locomotion i_locomotion)
		{
			if (i_locomotion.CharacterController.IsGrounded && m_remainingBoostTime <= 0.0f)
			{
				i_machine.SwitchToState(new LocomotingState(), i_locomotion);
			}
			if (i_locomotion.LocomotionControls.Jump == false)
			{
				i_machine.SwitchToState(new FreefallState(), i_locomotion);
			}
		}

		public override void FixedUpdate(Locomotion i_locomotion)
		{
			m_remainingBoostTime -= Time.fixedDeltaTime;

			Vector3 velWS = i_locomotion.GetBaseVelocityIntentionWS();
			i_locomotion.ApplyStickMovement(ref velWS);

			if (m_remainingBoostTime > 0.0f)
			{
				i_locomotion.ApplyJumpBoost(ref velWS, 1.0f - (m_remainingBoostTime / i_locomotion.m_jumpBoostTime));
			}

			i_locomotion.ConfirmVelocity(velWS);
			i_locomotion.UpdatePlayerLook();
		}

		private float m_remainingBoostTime;
	}
}