using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class QuickShutterButton : MonoBehaviour
	{
		public PhotoSystem m_photoSystem;
		public Camera m_camera;

		// Update is called once per frame
		void Update()
		{
			if (Input.GetButtonDown("Fire1"))
			{
				m_photoSystem.TakePhoto(m_camera);
			}
		}
	}
}