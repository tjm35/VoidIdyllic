using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HutongGames.PlayMaker;

namespace Totality.PlayMakerIntegration.InputSystem
{
	[ActionCategory("InputSystem")]
	public class GetButtonDown : FsmStateAction
	{
		[RequiredField]
		public InputActionReference buttonName;

		public FsmEvent sendEvent;

		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;
		
		public override void Reset()
		{
			buttonName = null;
			sendEvent = null;
			storeResult = null;
		}

		public override void OnEnter()
		{
			var action = buttonName.action;
			if (action != null)
			{
				action.started += OnPress;
			}
		}

		public override void OnExit()
		{
			var action = buttonName.action;
			if (action != null)
			{
				action.started -= OnPress;
			}
		}

		private void OnPress(InputAction.CallbackContext i_context)
		{
			m_pressed = true;
		}

		public override void OnUpdate()
		{
			if (m_pressed)
			{
			    Fsm.Event(sendEvent);
			}
			
			storeResult.Value = m_pressed;

			m_pressed = false;
		}

		private bool m_pressed = false;
	}
}