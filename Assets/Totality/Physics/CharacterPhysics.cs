using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Totality
{
	/// <summary>
	/// Implementation of ICharacterPhysics using Unity Physics.
	/// </summary>
	public class CharacterPhysics_Unity : ICharacterPhysics
	{
		#region ICharacterPhysics Impl
		// See ICharacterPhysics
		public bool Deembed(Object i_collider, ref Vector3 io_delta, CharacterPhysicsTypes.DeembedReaction i_reaction = null, int i_layerMask = Physics.AllLayers, int i_deembedRepeats = 3)
		{
			// Argument preconditions
			Debug.Assert(i_collider is Collider);

			Collider collider = (Collider)i_collider;
			bool deembedPerformed = false;
			int repeats = 0;
			while (repeats < i_deembedRepeats)
			{
				repeats++;
				Collider[] colliders = Physics.OverlapSphere(collider.bounds.center + io_delta, collider.bounds.extents.magnitude, i_layerMask, QueryTriggerInteraction.Ignore);
				if (colliders.GetLength(0) > 0)
				{
					float deepestDistance = 0.0f;
					Vector3 deepestDirection = Vector3.zero;
					int hitCount = 0;
					foreach (Collider c in colliders)
					{
						if (c == collider)
						{
							continue;
						}
						Vector3 direction;
						float distance;
						bool hit = Physics.ComputePenetration(collider, collider.transform.position + io_delta, collider.transform.rotation, c, c.transform.position, c.transform.rotation, out direction, out distance);
						if (hit)
						{
							if (distance > deepestDistance)
							{
								deepestDistance = distance;
								deepestDirection = direction;
								if (i_reaction != null)
								{
									i_reaction(c, direction);
								}
							}
							hitCount++;
						}
					}
					if (deepestDistance > 0.0f)
					{
						deembedPerformed = true;
						io_delta += deepestDistance * deepestDirection;
					}
					else
					{
						break;
					}
				}
				else
				{
					break;
				}
			}

			return deembedPerformed;
		}

		// See ICharacterPhysics
		public bool ProbeGround(out Vector3 o_groundPos, out RaycastHit o_hitInfo, Vector3 i_probeTop, Vector3 i_probeBottom, float i_radius, float i_groundSlopeLimit, int i_layerMask = Physics.AllLayers, System.Func<RaycastHit, bool> i_hitFilter = null)
		{
			o_groundPos = i_probeTop;
			o_hitInfo = new RaycastHit();
			System.Func<RaycastHit, bool> hitFilter = i_hitFilter ?? DefaultHitFilter;

			Vector3 up = (i_probeTop - i_probeBottom).normalized;

			Vector3 sphereStartPos = i_probeTop + i_radius * up;
			float maxDistance = (i_probeTop - i_probeBottom).magnitude;
			IEnumerable<RaycastHit> hitInfos = Physics.SphereCastAll(sphereStartPos, i_radius, -up, maxDistance, i_layerMask, QueryTriggerInteraction.Ignore).Where(hitFilter);
			Debug.DrawRay(sphereStartPos, -up * maxDistance, hitInfos.Any() ? Color.green : Color.red);
			if (hitInfos.Count() > 0)
			{
				o_hitInfo = hitInfos.First();
				if (o_hitInfo.distance > 0.0f)
				{
					// Determine base-of-sphere position from hit position.
					o_groundPos = o_hitInfo.point + i_radius * o_hitInfo.normal - i_radius * up;

					// Raycast at this point so we don't mistake a nearby wall for the ground.
					float groundMatchAdjust = 0.01f;
					IEnumerable<RaycastHit> rayHitInfos = Physics.RaycastAll(o_groundPos + groundMatchAdjust * up, -up, 2.0f * groundMatchAdjust, i_layerMask, QueryTriggerInteraction.Ignore).Where(hitFilter);
					if (rayHitInfos.Count() > 0)
					{
						o_hitInfo = rayHitInfos.First();
					}

					bool standAngleOkay = Vector3.Angle(o_hitInfo.normal, up) < i_groundSlopeLimit;
					return standAngleOkay;
				}
				else
				{
					// Sphere sweep has started out embedded; this shouldn't happen, but our safest option is to fail to find ground.
					return false;
				}
			}
			else
			{
				return false;
			}
		}

		// See ICharacterPhysics
		public Object MakeCharacterCollider(Transform i_parent, string i_name = "CharacterPhysics Collider")
		{
			GameObject colliderGO = new GameObject(i_name);
			CapsuleCollider collider = colliderGO.AddComponent<CapsuleCollider>();
			collider.center = Vector3.zero;
			collider.direction = 2; // Orient the capsule along forwards, so it's easier to orient with the transform.
			collider.transform.parent = i_parent;

			// TODO - Does this work? The rigidbody might need to be on the player gameobject, not the collider child.
			Rigidbody rb = colliderGO.AddComponent<Rigidbody>();
			rb.isKinematic = true;
			rb.constraints = RigidbodyConstraints.FreezeAll;

			return collider;
		}

		// See ICharacterPhysics
		public void ConfigureCharacterCollider(Object i_collider, float i_radius, float i_height, Vector3? i_upAxis = null, Vector3? i_offset = null)
		{
			// Default arguments
			Vector3 upAxis = i_upAxis.GetValueOrDefault(Vector3.up);
			Vector3 offset = i_offset.GetValueOrDefault(Vector3.zero);

			// Argument preconditions
			Debug.Assert(i_collider is CapsuleCollider);

			CapsuleCollider collider = (CapsuleCollider)i_collider;
			collider.radius = i_radius;
			collider.transform.localRotation = Quaternion.LookRotation(upAxis);
			collider.height = i_height;
			collider.transform.localPosition = 0.5f * i_height * upAxis + offset;
		}
		#endregion

		#region Private Functions
		private static bool DefaultHitFilter(RaycastHit _) { return true; }
		#endregion
	}
}