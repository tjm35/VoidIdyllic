using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality.StateMachine;

namespace Moonshot.Player.Locomotion
{
	public class FreefallState : State<Locomotion>
	{
		public override void UpdateState(StateMachine<Locomotion> i_machine, Locomotion i_locomotion)
		{
			if (i_locomotion.CharacterController.IsGrounded)
			{
				i_machine.SwitchToState(new LocomotingState(), i_locomotion);
			}
		}

		public override void FixedUpdate(Locomotion i_locomotion)
		{
			//i_locomotion.UpdateDragTurning();
			i_locomotion.UpdateStickMovement(true);

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
	}
}