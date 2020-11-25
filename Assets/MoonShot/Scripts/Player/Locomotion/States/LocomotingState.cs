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
			Vector3 velWS = i_locomotion.GetBaseVelocityIntentionWS();
			i_locomotion.ApplyStickMovement(ref velWS);
			i_locomotion.ConfirmVelocity(velWS);
			i_locomotion.UpdatePlayerLook();
		}
	}
}