using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using HutongGames.PlayMaker;

namespace Totality.PlayMakerIntegration.InputSystem
{
	[ActionCategory("InputSystem")]
	public class ActivateActionMap : FsmStateAction
	{
		[RequiredField]
		public InputActionAsset inputAsset;
		[RequiredField]
		public string actionMap;

		public FsmBool active;
		public FsmBool reverseOnExit;
		
		public override void Reset()
		{
			inputAsset = null;
			actionMap = "";
			active = true;
			reverseOnExit = true;
		}

		public override void OnEnter()
		{
			SetActive(active.Value);
		}

		public override void OnExit()
		{
			if (reverseOnExit.Value)
			{
				SetActive(!active.Value);
			}
		}

		private InputActionMap InputActionMap => inputAsset.FindActionMap(actionMap);

		private void SetActive(bool i_active)
		{ 
			if (i_active)
			{
				InputActionMap.Enable();
			}
			else if (!i_active)
			{
				InputActionMap.Disable();
			}
		}
	}
}