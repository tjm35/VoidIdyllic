using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Moonshot.UI
{
	public class MouseSensitivitySetting : MonoBehaviour
	{
		public float SensitivityPower
		{
			get
			{
				return m_sensitivityPower;
			}

			set
			{
				m_sensitivityPower = value;
				UpdateSensitivity();
			}
		}

		private void UpdateSensitivity()
		{
			s_sensitivity = Mathf.Pow(2.0f, m_sensitivityPower);
		}

		private static float s_sensitivity = 1;

		private float m_sensitivityPower = 0;

		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void RegisterSensitivityProcessor()
		{
			InputSystem.RegisterProcessor<SensitivityProcessor>();
		}

		public class SensitivityProcessor : InputProcessor<Vector2>
		{
			public override Vector2 Process(Vector2 value, InputControl control)
			{
				return value * s_sensitivity;
			}
		}
	}
}