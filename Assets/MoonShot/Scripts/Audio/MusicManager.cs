using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Moonshot.Planet;
using Moonshot.Player;
using Moonshot.World;
using UnityEngine;
using System;

namespace Moonshot.Audio
{
	[RequireComponent(typeof(AudioSource))]
	public class MusicManager : MonoBehaviour
	{
		[Serializable]
		public class PlanetaryMusicSetup
		{
			public OrreryPlanet m_planet;
			public AudioClip m_music;
		}

		[ReorderableList]
		public List<PlanetaryMusicSetup> m_planetMusics;

		public float m_minDelayTime = 1.0f;
		public float m_maxDelayTime = 5.0f;

		public float m_fadeInTime = 1.0f;
		public float m_fadeOutTime = 1.0f;

		// Start is called before the first frame update
		void Start()
		{
			m_source = GetComponent<AudioSource>();
		}

		// Update is called once per frame
		void Update()
		{
			switch (m_state)
			{
				case State.NotPlaying:
				default:
					{
						if (IsOnPlanet() && CurrentFramePlanetHasMusic())
						{
							m_delayTime = UnityEngine.Random.Range(m_minDelayTime, m_maxDelayTime);
							m_state = State.WaitingToStart;
						}
						break;
					}
				case State.WaitingToStart:
					{
						if (IsOnPlanet() && CurrentFramePlanetHasMusic())
						{
							m_delayTime -= Time.deltaTime;
							if (m_delayTime < 0.0f)
							{
								Play();
							}
						}
						else
						{
							m_state = State.NotPlaying;
						}
						break;
					}
				case State.Playing:
					{
						if (m_fadeInTime > 0.0f)
						{
							m_source.volume = Mathf.Min(m_source.volume + Time.deltaTime / m_fadeInTime, 1.0f);
						}
						if (m_currentMusic != GetMusicForCurrentFramePlanet())
						{
							Stop();
						}
						break;
					}
				case State.Stopping:
					{
						m_source.volume = Mathf.Max(m_source.volume - Time.deltaTime / m_fadeOutTime, 0.0f);
						if (m_source.volume <= 0.0f)
						{
							m_source.Stop();
							m_state = State.NotPlaying;
						}
						break;
					}
			}
		}

		private void Stop()
		{
			if (m_fadeOutTime > 0.0f)
			{
				m_state = State.Stopping;
			}
			else
			{
				m_source.Stop();
				m_source.clip = null;
			}
		}

		private void Play()
		{
			PlanetaryMusicSetup pm = GetMusicForCurrentFramePlanet();
			Debug.Assert(pm != null);

			m_source.clip = pm.m_music;
			if (m_fadeInTime > 0.0f)
			{
				m_source.volume = 0.0f;
			}
			else
			{
				m_source.volume = 1.0f;
			}
			m_source.Play();

			m_state = State.Playing;
			m_currentMusic = pm;
		}

		private bool CurrentFramePlanetHasMusic()
		{
			return GetMusicForCurrentFramePlanet() != null;
		}

		private PlanetaryMusicSetup GetMusicForCurrentFramePlanet()
		{
			if (PlayerVehicle.Current)
			{
				LocalFrame lf = LocalFrame.Get(PlayerVehicle.Current.transform);
				if (lf)
				{
					OrreryPlanet currentPlanet = lf.GlobalLocation.GetComponent<OrreryPlanet>();
					return m_planetMusics.Find(pm => pm.m_planet == currentPlanet);
				}
			}
			return null;
		}

		private bool IsOnPlanet()
		{
			return PlayerVehicle.Current && PlayerVehicle.Current.m_type == PlayerVehicle.VehicleType.OnFoot;
		}

		private enum State
		{
			NotPlaying,
			WaitingToStart,
			Playing,
			Stopping,
		}

		private AudioSource m_source;
		[ReadOnly]
		[SerializeField]
		private State m_state = State.NotPlaying;
		private PlanetaryMusicSetup m_currentMusic;
		private float m_delayTime = 0.0f;
	}
}