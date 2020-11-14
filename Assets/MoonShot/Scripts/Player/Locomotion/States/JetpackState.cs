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

			//i_locomotion.UpdateDragTurning();
			i_locomotion.UpdateStickMovement(false, m_remainingBoostTime > 0.0f);

			//i_fpc.FixedUpdatePlayerLook();
			//i_fpc.FixedUpdatePlayerMove(i_fpc.Cfg.m_inAirMoveAccel);
			//i_fpc.FixedUpdateCrouching(true);

			//if (CanJump(i_fpc) && i_fpc.PlayerControls.PullJump())
			//{
			//	i_fpc.FixedUpdateDoJump();
			//}

			//i_fpc.DSt.debug_canMantle = i_fpc.CanMantle();
			//if (i_fpc.PlayerControls.Jump && i_fpc.CanMantle())
			//{
			//	i_fpc.FixedUpdateApplyFalling(true);
			//	i_fpc.FixedUpdateDoMantling();
			//}
			//else
			//{
			//	i_fpc.FixedUpdateApplyFalling(false);
			//}
		}

		private float m_remainingBoostTime;
	}
}