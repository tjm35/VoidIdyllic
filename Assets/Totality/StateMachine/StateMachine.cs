using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Totality.StateMachine
{
	public class StateMachine<Context>
	{
		public StateMachine(State<Context> i_initialState, Context i_context)
		{
			SwitchToState(i_initialState, i_context);
		}

		public void Update(Context i_context)
		{
			UpdateState(i_context);
			m_state.FixedUpdate(i_context);
		}

		public void FixedUpdate(Context i_context)
		{
			UpdateState(i_context);
			m_state.FixedUpdate(i_context);
		}

		public void SwitchToState(State<Context> i_newState, Context i_context)
		{
			if (m_state != null)
			{
				m_state.ExitState(i_context);
			}
			m_state = i_newState;
			m_state.EnterState(i_context);
		}

		public State<Context> CurrentState => m_state;

		private void UpdateState(Context i_context)
		{
			// Repeat state migration until it stabalises; abort and emit a warning if we spend too long.
			State<Context> oldState;
			int switchCount = 0;
			do
			{
				switchCount++;
				oldState = m_state;
				m_state.UpdateState(this, i_context);
			} while (m_state != oldState && switchCount < 1000);
			Debug.Assert(switchCount < 100);
		}

		private State<Context> m_state;
	}
}