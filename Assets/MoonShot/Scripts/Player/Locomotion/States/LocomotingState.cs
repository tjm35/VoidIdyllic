using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality.StateMachine;

namespace Moonshot.Player.Locomotion
{
	public class LocomotingState : State<Locomotion>
	{
		public override void UpdateState(StateMachine<Locomotion> i_machine, Locomotion i_locomotion)
		{
			if (!i_locomotion.CharacterController.IsGrounded)
			{
				i_machine.SwitchToState(new FreefallState(), i_locomotion);
			}
			if (i_locomotion.LocomotionControls.PullJump())
			{
				i_machine.SwitchToState(new JetpackState(true, i_locomotion), i_locomotion);
			}
		}

		public override void FixedUpdate(Locomotion i_locomotion)
		{
			//i_locomotion.UpdateDragTurning();
			i_locomotion.UpdateStickMovement(false, false);

			//i_fpc.FixedUpdatePlayerLook();
			//i_fpc.FixedUpdatePlayerMove(i_fpc.Cfg.m_moveAccel);
			//i_fpc.FixedUpdateApplyFalling(true);
			//i_fpc.FixedUpdateCrouching(true);

			//if (i_fpc.PlayerControls.PullJump())
			//{
			//	i_fpc.FixedUpdateDoJump();
			//}

		}
	}
}