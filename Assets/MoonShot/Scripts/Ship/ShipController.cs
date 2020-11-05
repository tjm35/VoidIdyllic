using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Moonshot.World;
using Totality;

namespace Moonshot.Ship
{

	public class ShipController : MonoBehaviour
	{
		public string[] m_collisionLayers;
		public float m_shipCollideForceMultiplier = 20.0f;

		public Vector3 WorldVelocity { get; set; }

		void Start()
		{
			//m_gravityProvider = m_gravityProviderObject?.GetComponent<IGravityProvider>();
			m_characterPhysics = GetComponent<ICharacterPhysics>();
			if (m_characterPhysics == null)
			{
				m_characterPhysics = new CharacterPhysics_Unity();
			}
			m_layerMask = LayerMask.GetMask(m_collisionLayers);

			m_collider = GetComponent<Collider>();
		}

		// Update is called once per frame
		void FixedUpdate()
		{
			// Move due to velocity
			Vector3 delta = WorldVelocity * Time.fixedDeltaTime;
			//DebugCheckDistance(delta, "m_velocity");
			m_characterPhysics.Deembed(m_collider, ref delta, DoDeembedReaction, m_layerMask);
			//DebugCheckDistance(delta, "DeembedCharacter");
			Vector3 newPos = transform.position + delta;
			
			transform.position = newPos;
		}

		public void OnExitLocalFrame(LocalFrame i_frame)
		{
			WorldVelocity = i_frame.TransformVectorToGlobal(WorldVelocity);
		}

		public void OnEnterLocalFrame(LocalFrame i_frame)
		{
			WorldVelocity = i_frame.TransformVectorToLocal(WorldVelocity);
		}

		private void DoDeembedReaction(Object i_collider, Vector3 i_deembedDirection)
		{
			Collider collider = i_collider as Collider;
			if (collider && collider.attachedRigidbody)
			{
				collider.attachedRigidbody.AddForceAtPosition(m_shipCollideForceMultiplier * Vector3.Dot(WorldVelocity - collider.attachedRigidbody.velocity, i_deembedDirection) * i_deembedDirection, transform.position);
			}
			SendMessage("OnShipCollided", i_collider);
		}

		private int m_layerMask;
		private ICharacterPhysics m_characterPhysics;
		private Collider m_collider;
	}
}