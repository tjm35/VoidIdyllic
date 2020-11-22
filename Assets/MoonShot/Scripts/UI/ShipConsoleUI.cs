using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Moonshot.Ship;

namespace Moonshot.UI
{
	public class ShipConsoleUI : MonoBehaviour
	{
		public Color m_zeroColor;
		public Color m_lowColor;
		public Color m_highColor;

		public ShipController m_shipController;
		public ShipMovement m_shipMovement;
		public ShipControls m_shipControls;
		public float m_velocityDisplayScale = 1.0f;

		public Image m_leftPad;
		public Image m_rightPad;
		public Image m_thrustImage;
		public RectTransform m_radialThrustHook;
		public Image m_radialThrustImage;

		public Transform m_spectrumVisualisers;
		public float m_visualiserMaxHeight = 80.0f;
		public float m_analyserFrequency = 1.0f;

		private void Start()
		{
			if (m_spectrumVisualisers)
			{
				m_analysers = new List<(RectTransform, Graphic, float)>();
				for (int i = 0; i < m_spectrumVisualisers.childCount; ++i)
				{
					m_analysers.Add((m_spectrumVisualisers.GetChild(i).GetComponent<RectTransform>(), m_spectrumVisualisers.GetChild(i).GetComponent<Graphic>(), Random.Range(0.0f, 2.0f * Mathf.PI)));
				}
			}
		}

		// Update is called once per frame
		void Update()
		{
			// Pad activity.
			{
				float leftActivity = m_shipControls.Move.magnitude;
				float rightActivity = m_shipControls.Look.magnitude;

				SmoothUpdateColor(m_leftPad, Color.Lerp(m_zeroColor, m_lowColor, leftActivity));
				SmoothUpdateColor(m_rightPad, Color.Lerp(m_zeroColor, m_lowColor, rightActivity));
			}

			// Thrust image based on velocity
			if (false)
			{
				Vector3 velocity = m_shipController.transform.InverseTransformDirection(m_shipController.WorldVelocity);
				Vector2 horizVelocity = new Vector2(velocity.x, velocity.z);

				m_shipMovement.GetMaxSpeed(out float maxSpeed);

				float totalSpeedScaled = Mathf.Clamp01(velocity.magnitude / maxSpeed) * m_velocityDisplayScale;
				float horizSpeedScaled = Mathf.Clamp01(horizVelocity.magnitude / maxSpeed) * m_velocityDisplayScale;
				float horizAngle = Vector2.SignedAngle(horizVelocity, Vector2.up);

				m_radialThrustHook.localRotation = Quaternion.Euler(0.0f, 0.0f, horizAngle);
				m_radialThrustHook.localScale = (0.6f * Mathf.Clamp01(horizSpeedScaled) + 0.4f) * Vector3.one;
				m_radialThrustImage.color = Color.Lerp(m_zeroColor, m_lowColor, horizSpeedScaled);

				m_thrustImage.color = Color.Lerp(m_zeroColor, m_highColor, totalSpeedScaled);
			}

			// Thrust image based on thrust
			if (true)
			{
				Vector3 thrust = m_shipControls.Move;
				Vector2 horizThrust = new Vector2(-thrust.x, thrust.z);

				float horizAngle = Vector2.SignedAngle(horizThrust, Vector2.down);

				m_radialThrustHook.localRotation = Quaternion.Euler(0.0f, 0.0f, horizAngle);
				m_radialThrustHook.localScale = (0.6f * Mathf.Clamp01(horizThrust.magnitude) + 0.4f) * Vector3.one;
				SmoothUpdateColor(m_radialThrustImage, Color.Lerp(m_zeroColor, m_lowColor, horizThrust.magnitude));

				SmoothUpdateColor(m_thrustImage, Color.Lerp(m_zeroColor, m_highColor, thrust.magnitude));
			}

			if (m_analysers != null)
			{
				Vector3 velocity = m_shipController.transform.InverseTransformDirection(m_shipController.WorldVelocity);

				m_shipMovement.GetMaxSpeed(out float maxSpeed);

				float totalSpeedScaled = Mathf.Clamp01(velocity.magnitude / maxSpeed) * m_velocityDisplayScale;

				for (int i = 0; i < m_analysers.Count; ++i)
				{
					float newHeight = Mathf.Abs(Mathf.Sin((Time.timeSinceLevelLoad * m_analyserFrequency) + m_analysers[i].m_phase)) * totalSpeedScaled * m_visualiserMaxHeight;
					m_analysers[i].m_rect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, newHeight);
					m_analysers[i].m_graphic.color = Color.Lerp(m_lowColor, m_highColor, newHeight / m_visualiserMaxHeight);
				}
			}

			m_firstFrame = false;
		}

		private void SmoothUpdateColor(Graphic i_targetImage, Color i_targetColor)
		{
			i_targetImage.color = Color.Lerp(i_targetImage.color, i_targetColor, 0.2f);
			if (m_firstFrame)
			{
				i_targetImage.color = i_targetColor;
			}
		}

		private bool m_firstFrame = true;
		private List<(RectTransform m_rect, Graphic m_graphic, float m_phase)> m_analysers;
	}
}