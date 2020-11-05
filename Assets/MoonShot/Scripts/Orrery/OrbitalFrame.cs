using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Orrery
{
	[ExecuteAlways]
	public class OrbitalFrame : MonoBehaviour
	{
		public Vector3 m_axis = Vector3.up;
		public float m_period = 10.0f;
		public float m_eccentricity = 0.0f;
		public Vector3 m_semimajorAxis = 100.0f * Vector3.right;
		public float m_offsetAngle = 0.0f;

		void Start()
		{
			Transform t = transform;
			while (t != null && m_timeSource == null)
			{
				m_timeSource = t.GetComponent<OrreryTimeSource>();
				t = t.parent;
			}

			m_Elookup = GenerateELookup(m_eccentricity);
		}

		public void Update()
		{
			float t = m_timeSource?.TimeElapsed ?? 0.0f;

			float n = 2.0f * Mathf.PI / m_period;
			float M = WrapAngleRadians((n * t) + (Mathf.Deg2Rad * m_offsetAngle) + Mathf.PI) - Mathf.PI;
			float E = CalcEccentricAnomaly(M);
			float temp = Mathf.Tan(E / 2.0f);
			float theta = Mathf.Sign(M) * 2.0f * (Mathf.Atan(Mathf.Sqrt((1 + m_eccentricity) * temp * temp / (1 + m_eccentricity))));
			Vector3 radial = m_semimajorAxis * (1.0f - (m_eccentricity * Mathf.Cos(E)));

			Vector3 pos = Quaternion.AngleAxis(theta * Mathf.Rad2Deg, m_axis) * radial;

			transform.localPosition = pos;
		}

		private float WrapAngleRadians(float angle)
		{
			float turns = angle / (2.0f * Mathf.PI);
			float localTurns = turns - Mathf.Floor(turns);
			return localTurns * 2.0f * Mathf.PI;
		}

		private float CalcEccentricAnomaly(float M)
		{
			float[] Elookup = m_Elookup;
			if (Elookup == null || Elookup.Length == 0)
			{
				Elookup = GenerateELookup(m_eccentricity);
			}

			float lookupAddress = WrapAngleRadians(M) * Elookup.Length / (2.0f * Mathf.PI);
			lookupAddress = Mathf.Clamp(lookupAddress, 0.0f, Elookup.Length - 1.0001f);

			int lookupAddressLow = Mathf.FloorToInt(lookupAddress);
			float lookupAddressRemainder = lookupAddress - lookupAddressLow;

			return Mathf.Lerp(Elookup[lookupAddressLow], Elookup[lookupAddressLow + 1], lookupAddressRemainder);
			/*
			if (Mathf.Approximately(e, 1.0f))
			{
				float s = Mathf.Pow(6 * M, 1 / 3);
				return 
					s
					+ (s * s * s / 60)
					;
			}
			else
			{
				return
					(M / (1 - e))
					- (M * M * M * e / ((1 - e) * (1 - e) * (1 - e) * (1 - e) * 3 * 2))
					+ (M * M * M * M * M * (9 * e * e + e) / ((1 - e) * (1 - e) * (1 - e) * (1 - e) * (1 - e) * (1 - e) * (1 - e) * 5 * 4 * 3 * 2))
					- (M * M * M * M * M * M * M * (225 * e * e * e + 54 * e * e + e) / ((1 - e) * (1 - e) * (1 - e) * (1 - e) * (1 - e) * (1 - e) * (1 - e) * 5 * 4 * 3 * 2))
					;
			}
			*/
		}

		private static float[] GenerateELookup(float eccentricity)
		{
			
			int sampleCount = 360;
			float sampleRate = 2.0f * Mathf.PI / (float)sampleCount;
			float[] Ms = new float[sampleCount];

			for (int i = 0; i < sampleCount; ++i)
			{
				float E = (float)i * sampleRate;
				Ms[i] = E - eccentricity * Mathf.Sin(E);
			}

			int storedSampleCount = 360;
			float storedSampleRate = 2.0f * Mathf.PI / (float)storedSampleCount;
			float[] Elookup = new float[storedSampleCount];
			int lookAddress = 0;
			float latestM = Ms[0];
			for (int i = 0; i < storedSampleCount; ++i)
			{
				float M = (float)i * storedSampleRate;
				while (latestM < M && lookAddress < sampleCount - 1)
				{
					lookAddress++;
					latestM = Ms[lookAddress];
				}
				float prevM = lookAddress > 0 ? Ms[lookAddress - 1] : Ms[0];
				float interpAddress = (float)lookAddress - 1 + Mathf.InverseLerp(prevM, latestM, M);
				float interpE = interpAddress * sampleRate;

				Elookup[i] = interpE;
			}

			return Elookup;
		}

		private OrreryTimeSource m_timeSource;
		private float[] m_Elookup;
	}
}