using Moonshot.Player;
using Moonshot.World;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class SunRoarControl : MonoBehaviour
	{
		public Transform SunBody;
		public float m_falloffNear = 300.0f;
		public float m_falloffFar = 600.0f;

		private void Start()
		{
			m_source = GetComponent<AudioSource>();
		}

		private void Update()
		{
			var playerPos = LocalFrame.GetGlobalPosition(PlayerVehicle.Current.transform);
			var sunPos = LocalFrame.GetGlobalPosition(SunBody);
			var distance = Vector3.Distance(playerPos, sunPos);

			float volume = 1.0f - Mathf.Clamp01(Mathf.InverseLerp(m_falloffNear, m_falloffFar, distance));

			m_source.volume = volume;
		}

		private AudioSource m_source;
	}
}