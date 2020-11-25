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

			float speedProp = velWS.magnitude / i_locomotion.m_speed;
			if (speedProp > 0.05f)
			{
				m_footfallTime += Time.fixedDeltaTime;
				if (m_footfallTime * speedProp > i_locomotion.m_footfallRate)
				{
					m_footfallTime -= i_locomotion.m_footfallRate;
					i_locomotion.LocomotionAudio.DoFootfall(i_locomotion.CharacterController.LastGroundMaterial, speedProp);
				}
			}
		}

		private float m_footfallTime = 0.0f;
	}
}