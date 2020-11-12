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
		public float m_switchSpaceFrameDistance = 100.0f;

		public float m_shiftDiscrepancyWarningThreshold = 0.01f;

		void Start()
		{
			UpdateFrame(true);
		}

		void FixedUpdate()
		{
			UpdateFrame(false);
		}

		void UpdateFrame(bool i_instant)
		{
			LocalFrame currentFrame = LocalFrame.Get(transform);

			if (ShouldSwitchFrames(currentFrame))
			{
				var oldGlobalPos = LocalFrame.TransformPointToGlobal(currentFrame, transform.position);

				LocalFrame newFrame = CreateSuitableLocalFrame(currentFrame);
				ExitAndDestroyFrame(currentFrame);

				var midGlobalPos = transform.position;

				EnterFrame(newFrame);

				var newGlobalPos = LocalFrame.TransformPointToGlobal(newFrame, transform.position);

				var oldMidShift = (midGlobalPos - oldGlobalPos).magnitude;
				var oldNewShift = (newGlobalPos - oldGlobalPos).magnitude;

				//Debug.Log($"PlayerFrameManager: Frame shift occurred! Position errors {oldMidShift}, {oldNewShift}");
				Debug.Assert(oldNewShift < m_shiftDiscrepancyWarningThreshold, "PlayerFrameManager: Unacceptably large error when shifting frames.");
			}
		}

		private bool ShouldSwitchFrames(LocalFrame currentFrame)
		{
			if (currentFrame == null)
			{
				return true;
			}

			var idealFrame = GetIdealFramePlanet(currentFrame);

			if (idealFrame == null && !currentFrame.GlobalLocationIsTemporary)
			{
				// We shouldn't be in a planet frame any more.
				return true;
			}

			if (idealFrame != null && currentFrame.GlobalLocation != idealFrame.transform)
			{
				// We should switch to this ideal frame.
				return true;
			}

			if (idealFrame == null && currentFrame.GlobalLocationIsTemporary)
			{
				// We might want to change our space frame?
				Vector3 globalPos = LocalFrame.TransformPointToGlobal(currentFrame, transform.position);
				Vector3 framePos = currentFrame.GlobalLocation.position;
				if ((globalPos - framePos).sqrMagnitude > m_switchSpaceFrameDistance * m_switchSpaceFrameDistance)
				{
					return true;
				}
			}

			return false;
		}

		private OrreryPlanet GetIdealFramePlanet(LocalFrame currentFrame)
		{
			IEnumerable<OrreryPlanet> planets = FindObjectsOfType<OrreryPlanet>();

			Vector3 globalPos = LocalFrame.TransformPointToGlobal(currentFrame, transform.position);
			var nearestPlanet = planets.Where(p => p.CanConstructFrame).OrderBy(p => (p.transform.position - globalPos).magnitude - p.Radius).FirstOrDefault();

			if (nearestPlanet)
			{
				float distanceToSurface = (nearestPlanet.transform.position - globalPos).magnitude - nearestPlanet.Radius;

				if (distanceToSurface < m_planetFrameRange)
				{
					return nearestPlanet;
				}
			}

			return null;
		}

		private LocalFrame CreateSuitableLocalFrame(LocalFrame currentFrame)
		{
			var nearestPlanet = GetIdealFramePlanet(currentFrame);

			if (nearestPlanet)
			{
				var newFrame = nearestPlanet.ConstructLocalFrame();
				if (newFrame)
				{
					return newFrame;
				}
			}

			return ConstructSpaceFrame(currentFrame);
		}

		private LocalFrame ConstructSpaceFrame(LocalFrame currentFrame)
		{
			Vector3 globalPos = LocalFrame.TransformPointToGlobal(currentFrame, transform.position);

			GameObject globalHook = new GameObject("Temporary Global Hook");
			globalHook.transform.position = globalPos;
			// TODO: Construct suitable orbital frame.

			GameObject frameObject = new GameObject("Space Local Frame");
			var frame = frameObject.AddComponent<LocalFrame>();
			frame.GlobalLocation = globalHook.transform;
			frame.GlobalLocationIsTemporary = true;

			return frame;
		}

		private void ExitAndDestroyFrame(LocalFrame oldFrame)
		{
			if (oldFrame)
			{
				BroadcastMessage("OnExitLocalFrame", oldFrame);
				Vector3 globalPos = oldFrame.TransformPointToGlobal(transform.position);
				Quaternion globalRot = oldFrame.TransformRotationToGlobal(transform.rotation);
				transform.SetParent(null);
				transform.SetPositionAndRotation(globalPos, globalRot);
				if (oldFrame.GlobalLocationIsTemporary)
				{
					Destroy(oldFrame.GlobalLocation.gameObject);
				}
				Destroy(oldFrame.gameObject);
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
				BroadcastMessage("OnEnterLocalFrame", newFrame);
			}
		}
	}
}