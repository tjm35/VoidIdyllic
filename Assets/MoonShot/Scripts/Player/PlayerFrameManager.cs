using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.Planet;
using Moonshot.World;
using System.Linq;

namespace Moonshot.Player
{
	public class PlayerFrameManager : MonoBehaviour
	{
		public float m_planetFrameRange = 300.0f;

		void Start()
		{
			UpdateFrame(true);
		}

		void Update()
		{
			UpdateFrame(false);
		}

		void UpdateFrame(bool i_instant)
		{
			LocalFrame currentFrame = LocalFrame.Get(transform);

			if (ShouldSwitchFrames(currentFrame))
			{
				LocalFrame newFrame = CreateSuitableLocalFrame(currentFrame);
				ExitFrame(currentFrame);
				EnterFrame(newFrame);
				if (currentFrame)
				{
					Destroy(currentFrame);
				}
			}
		}

		private bool ShouldSwitchFrames(LocalFrame currentFrame)
		{
			// TODO - Other reasons
			return currentFrame == null;
		}

		private LocalFrame CreateSuitableLocalFrame(LocalFrame currentFrame)
		{
			IEnumerable<OrreryPlanet> planets = FindObjectsOfType<OrreryPlanet>();

			Vector3 globalPos = LocalFrame.TransformPointToGlobal(currentFrame, transform.position);
			var nearestPlanet = planets.Where(p => p.CanConstructFrame).OrderBy(p => (p.transform.position - globalPos).magnitude - p.Radius).FirstOrDefault();

			if (nearestPlanet)
			{
				float distanceToSurface = (nearestPlanet.transform.position - globalPos).magnitude - nearestPlanet.Radius;

				if (distanceToSurface < m_planetFrameRange)
				{
					var newFrame = nearestPlanet.ConstructLocalFrame();
					if (newFrame)
					{
						return newFrame;
					}
				}
			}

			return ConstructSpaceFrame();
		}

		private LocalFrame ConstructSpaceFrame()
		{
			// TODO
			return null;
		}

		private void ExitFrame(LocalFrame oldFrame)
		{
			if (oldFrame)
			{
				Vector3 globalPos = oldFrame.TransformPointToGlobal(transform.position);
				Quaternion globalRot = oldFrame.TransformRotationToGlobal(transform.rotation);
				transform.SetParent(null);
				transform.SetPositionAndRotation(globalPos, globalRot);
			}
		}

		private void EnterFrame(LocalFrame newFrame)
		{
			if (newFrame)
			{
				Vector3 localPos = newFrame.TransformPointToLocal(transform.position);
				Quaternion localRot = newFrame.TransformRotationToLocal(transform.rotation);
				transform.SetParent(newFrame.transform);
				transform.localPosition = localPos;
				transform.localRotation = localRot;
			}
		}
	}
}