using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality.StateMachine;

namespace Moonshot.Player.Locomotion
{
	public class InitialState : State<Locomotion>
	{
		public override void EnterState(Locomotion i_locomotion)
		{
			//i_fpc.ResetDynamicState();
		}
		public override void UpdateState(StateMachine<Locomotion> i_machine, Locomotion i_locomotion)
		{
			i_machine.SwitchToState(new LocomotingState(), i_locomotion);
		}
	}
}