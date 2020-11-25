using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Totality;
using Moonshot.World;

namespace Moonshot.Player
{
	public class CharacterController : MonoBehaviour
	{
		#region Editor Fields
		public string[] m_collisionLayers;
		public Vector3 m_up = Vector3.up;
		public float m_minHeight = 0.5f;
		public float m_maxHeight = 3.0f;
		public float m_radius = 0.5f;

		public float m_stepUpHeight = 0.1f;
		public float m_groundSnapHeight = -0.02f; // TODO - Should be an angle?
		public float m_slopeStandLimit = 45.0f;
		public float m_playerPushForceMultiplier = 2.0f;
		public float m_playerWeightForce = 20.0f;
		public float m_overheadRoom = 0.1f;

		public int debug_overlapHits = 0;
		public int debug_penetrationHits = 0;
		#endregion

		#region Public State
		public Vector3 Velocity { get; private set; } = Vector3.zero;
		public bool IsGrounded { get; private set; } = true;
		public PhysicMaterial LastGroundMaterial { get; private set; } = null;

		public Vector3 UpWS
		{
			get
			{
				return transform.TransformDirection(m_up);
			}
		}
		#endregion

		#region Public Methods
		public bool MoveLocalVelocity(Vector3 i_velocity)
		{
			Velocity = transform.TransformVector(i_velocity);
			return true;
		}

		public bool MoveWorldVelocity(Vector3 i_velocity)
		{
			Velocity = i_velocity;
			return true;
		}

		public bool ProbeGround(Vector3 i_characterPos, out Vector3 o_groundPos, out RaycastHit o_hitInfo, float i_fromHeight, float i_toHeight)
		{
			bool hitFound = m_characterPhysics.ProbeGround(out o_groundPos, out o_hitInfo, i_characterPos + i_fromHeight * UpWS, i_characterPos + i_toHeight * UpWS, m_radius, m_slopeStandLimit, m_layerMask, h => h.collider != m_collider);
			if (hitFound)
			{
				//bool standSurfaceOkay = o_hitInfo.GetExtraData() == null || o_hitInfo.GetExtraData().StableGround;
				//return standSurfaceOkay;
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool ProbeGround(Vector3 i_characterPos, out Vector3 o_groundPos, out RaycastHit o_hitInfo)
		{
			return ProbeGround(i_characterPos, out o_groundPos, out o_hitInfo, m_stepUpHeight, m_groundSnapHeight);
		}
		#endregion

		#region Unity Event Functions
		// See Unity Docs
		void Start()
		{
			m_gravityProvider = GlobalGravityProvider.Instance;
			m_characterPhysics = GetComponent<ICharacterPhysics>();
			if (m_characterPhysics == null)
			{
				m_characterPhysics = new CharacterPhysics_Unity();
			}
			m_layerMask = LayerMask.GetMask(m_collisionLayers);

			m_collider = m_characterPhysics.MakeCharacterCollider(transform);
			m_characterPhysics.ConfigureCharacterCollider(m_collider, m_radius, m_maxHeight, m_up);
		}

		// See Unity Docs
		void FixedUpdate()
		{
			FixedUpdateUp();

			FixedUpdateCrouching();

			// Move due to velocity
			Vector3 delta = Velocity * Time.fixedDeltaTime;
			DebugCheckDistance(delta, "m_velocity");
			m_characterPhysics.Deembed(m_collider, ref delta, DoDeembedReaction, m_layerMask);
			DebugCheckDistance(delta, "DeembedCharacter");
			Vector3 newPos = transform.position + delta;

			Vector3 groundPos;
			RaycastHit groundHitInfo;
			if (ProbeGround(newPos, out groundPos, out groundHitInfo))
			{
				IsGrounded = true;
				DebugCheckDistance(groundPos - newPos, "ProbeGround");

				newPos = groundPos;
				if (groundHitInfo.collider && groundHitInfo.collider.attachedRigidbody)
				{
					groundHitInfo.collider.attachedRigidbody.AddForceAtPosition(m_playerWeightForce * -UpWS, groundPos);
				}
				if (groundHitInfo.collider)
				{
					LastGroundMaterial = groundHitInfo.collider.sharedMaterial;
				}
				else
				{
					LastGroundMaterial = null;
				}
			}
			else
			{
				IsGrounded = false;
			}

			transform.position = newPos;
		}
		#endregion

		#region Private Functions

		void FixedUpdateUp()
		{
			if (m_gravityProvider != null)
			{
				Vector3 newUpWS;
				var localFrame = LocalFrame.Get(transform);
				if (m_gravityProvider.GetUp(LocalFrame.TransformPointToGlobal(localFrame, transform.position), out newUpWS))
				{
					newUpWS = LocalFrame.TransformVectorToLocal(localFrame, newUpWS);
					Vector3 adjustedForward = Vector3.Cross(transform.right, newUpWS);
					transform.rotation = Quaternion.LookRotation(adjustedForward, newUpWS);
				}
			}
		}

		void FixedUpdateCrouching()
		{
			//if (ShouldCrouch && !m_isCrouching)
			//{
			//	SetColliderCrouching();
			//}
			//if (!ShouldCrouch && m_isCrouching && HasStandingRoom())
			//{
			//	SetColliderStanding();
			//}
		}

		void DebugCheckDistance(Vector3 i_diff, string i_context)
		{
			float saneThreshold = 1.0f;
			if (i_diff.sqrMagnitude > saneThreshold * saneThreshold)
			{
				Debug.LogFormat("FirstPersonCharacterController: Unexpectedly large delta {0} in '{1}'.", i_diff.magnitude, i_context);
			}
		}

		private void DoDeembedReaction(Object i_collider, Vector3 i_deembedDirection)
		{
			Collider collider = i_collider as Collider;
			if (collider && collider.attachedRigidbody)
			{
				collider.attachedRigidbody.AddForceAtPosition(m_playerPushForceMultiplier * Vector3.Dot(Velocity, i_deembedDirection) * i_deembedDirection, transform.position);
			}
		}
		#endregion

		#region Private Data
		private int m_layerMask;
		private Object m_collider;
		private ICharacterPhysics m_characterPhysics;
		private IGravityProvider m_gravityProvider;
		#endregion
	}
}