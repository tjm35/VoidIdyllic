using Moonshot.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Player
{
	[RequireComponent(typeof(Camera))]
	public class UpdateFOVFromSetting : MonoBehaviour
	{
		// Start is called before the first frame update
		void Start()
		{
			m_camera = GetComponent<Camera>();
		}

		// Update is called once per frame
		void Update()
		{
			m_camera.fieldOfView = FOVSetting.s_FOV;
		}

		private Camera m_camera;
	}
}