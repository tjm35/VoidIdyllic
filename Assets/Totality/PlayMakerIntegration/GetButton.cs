using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HutongGames.PlayMaker;

namespace Totality.PlayMakerIntegration.InputSystem
{
	[ActionCategory("InputSystem")]
	public class GetButton : FsmStateAction
	{
		[RequiredField]
		public InputActionReference buttonName;

		[RequiredField]
		[UIHint(UIHint.Variable)]
		public FsmBool storeResult;
		
		public override void Reset()
		{
			buttonName = null;
			storeResult = null;
		}

		public override void OnUpdate()
		{
			bool isDown = buttonName.action.ReadValue<float>() > 0.5f;

			storeResult.Value = isDown;
		}
	}
}