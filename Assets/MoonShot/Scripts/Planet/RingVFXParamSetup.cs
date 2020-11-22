using Moonshot.Player;
using Moonshot.World;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.VFX;

namespace Moonshot.Planet
{
	[ExecuteAlways]
	[RequireComponent(typeof(VisualEffect))]
	public class RingVFXParamSetup : MonoBehaviour
	{
		[Required]
		public OrreryPlanet m_sun;

		public float m_inFlightColliderRadius = 15.0f;
		public float m_onFootColliderRadius = 3.0f;

		private void Start()
		{
			m_vfx = GetComponent<VisualEffect>();
			m_shadowCasterPosID = Shader.PropertyToID("ShadowCasterPos");
			m_colliderCenterID = Shader.PropertyToID("ColliderCenter");
			m_colliderRadiusID = Shader.PropertyToID("ColliderRadius");
		}

		private void LateUpdate()
		{
			LocalFrame frame = LocalFrame.Get(transform);
			m_vfx.SetVector3(m_shadowCasterPosID, transform.InverseTransformPoint(LocalFrame.TransformPointToLocal(frame, LocalFrame.GetGlobalPosition(m_sun.transform))));

			// Collider setup
			if (PlayerVehicle.Current)
			{
				float radius = (PlayerVehicle.Current.m_type == PlayerVehicle.VehicleType.InFlight) ? m_inFlightColliderRadius : m_onFootColliderRadius;
				m_vfx.SetFloat(m_colliderRadiusID, radius);

				Vector3 playerGlobalPos = LocalFrame.GetGlobalPosition(PlayerVehicle.Current.transform);
				m_vfx.SetVector3(m_colliderCenterID, transform.InverseTransformPoint(LocalFrame.TransformPointToLocal(frame, playerGlobalPos)));
			}
			else
			{
				m_vfx.SetFloat(m_colliderRadiusID, 0.0f);
			}
		}

		private VisualEffect m_vfx;
		private int m_shadowCasterPosID;
		private int m_colliderCenterID;
		private int m_colliderRadiusID;
	}
}