using Moonshot.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor;

namespace Moonshot.Editor
{
	[InitializeOnLoad]
	public class RegisterSensitivityProcessor : MonoBehaviour
	{
		static RegisterSensitivityProcessor()
		{
			InputSystem.RegisterProcessor<MouseSensitivitySetting.SensitivityProcessor>();
		}
	}
}
