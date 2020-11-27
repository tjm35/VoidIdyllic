using OVR;
using Moonshot.Ship;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Audio
{
	[RequireComponent(typeof(ShipController))]
	[RequireComponent(typeof(ShipControls))]
	public class ShipAudio : MonoBehaviour
	{
		public AudioSource m_shipIdleSource;
		public AudioSource m_shipEngineSource;
		public SoundFXRef m_coastFX;
		public float m_turnAudioEffect = 0.1f;
		public float m_blendRate = 0.1f;
		public float m_coastThresholdHigh = 0.9f;
		public float m_coastThresholdLow = 0.4f;
		public float m_idleVolumeAdjust = 0.6f;
		public float m_idleDucking = 0.5f;

		public bool Active { get; set; } = true;

		private void Start()
		{
			m_shipController = GetComponent<ShipController>();
			m_shipControls = GetComponent<ShipControls>();
			m_shipMovement = GetComponent<ShipMovement>();
		}

		private void Update()
		{
			float speed = m_shipController.WorldVelocity.magnitude;
			m_shipMovement.GetMaxSpeed(out float maxSpeed);
			float speedProp = Mathf.Clamp01(speed / maxSpeed);
			float accelProp = m_shipControls.Move.magnitude;
			float turnProp = Mathf.Clamp01(m_shipControls.Look.magnitude);

			float engineStrength = Mathf.Clamp01(accelProp + m_turnAudioEffect * turnProp);

			float targetVolume = Active ? 1.0f: 0.0f;

			m_smoothedEngineStrength = Mathf.Lerp(m_smoothedEngineStrength, engineStrength, m_blendRate);
			m_smoothedOverallVolume = Mathf.Lerp(m_smoothedOverallVolume, targetVolume, m_blendRate);

			m_shipEngineSource.volume = m_smoothedEngineStrength * m_smoothedOverallVolume;
			m_shipIdleSource.volume = (1.0f - m_idleDucking * m_smoothedEngineStrength) * m_smoothedOverallVolume * m_idleVolumeAdjust;

			if (m_smoothedEngineStrength > m_coastThresholdHigh)
			{
				m_hasPendingCoast = true;
			}
			if (m_smoothedEngineStrength < m_coastThresholdLow && m_hasPendingCoast)
			{
				AudioManager.PlaySound(m_coastFX.soundFX, volumeOverride: speedProp);
				m_hasPendingCoast = false;
			}
		}

		private float m_smoothedEngineStrength = 0.0f;
		private float m_smoothedOverallVolume = 0.0f;
		private bool m_hasPendingCoast = false;
		private ShipController m_shipController;
		private ShipControls m_shipControls;
		private ShipMovement m_shipMovement;
	}
}