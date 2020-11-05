using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	public class OrreryTimeSource : MonoBehaviour
	{
		public float TimeElapsed { get; private set; } = 0.0f;

		void Update()
		{
			TimeElapsed += Time.deltaTime;
		}
	}
}