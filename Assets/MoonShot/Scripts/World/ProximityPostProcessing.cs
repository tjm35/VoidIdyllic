using Moonshot.Player;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace Moonshot.World
{
	public class ProximityPostProcessing : MonoBehaviour
	{
		public Volume m_volume;
		public Transform m_point;
		public float m_minRadius;
		public float m_maxRadius;

		// Update is called once per frame
		void Update()
		{
			var playerLocation = LocalFrame.GetGlobalPosition(PlayerVehicle.Current.transform);
			var pointLocation = LocalFrame.GetGlobalPosition(m_point.transform);
			var distance = Vector3.Distance(playerLocation, pointLocation);
			var prop = 1.0f - Mathf.Clamp01(Mathf.InverseLerp(m_minRadius, m_maxRadius, distance));

			//UI.QuickDebug.Print($"pl: {playerLocation}, po: {pointLocation}, ds: {distance}, pr: {prop}");

			m_volume.enabled = (prop > 0.0f);
			m_volume.weight = prop;
		}
	}
}