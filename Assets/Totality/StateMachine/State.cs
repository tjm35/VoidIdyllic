using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality.StateMachine
{
	public abstract class State<Context>
	{
		public virtual void EnterState(Context i_context) { }

		public virtual void UpdateState(StateMachine<Context> i_machine, Context i_context) { }
		public virtual void Update(Context i_context) { }
		public virtual void FixedUpdate(Context i_context) { }

		public virtual void ExitState(Context i_context) { }
	}
}