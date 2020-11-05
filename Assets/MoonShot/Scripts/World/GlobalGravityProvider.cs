using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	public class GlobalGravityProvider : MonoBehaviour, IGravityProvider
	{
		static public GlobalGravityProvider Instance { get; private set; }

		public IList<IGravityProvider> PlanetProviders => m_planetProviders;

		void Awake()
		{
			Debug.Assert(Instance == null);
			Instance = this;
		}

		void OnDestroy()
		{
			Debug.Assert(Instance == this);
			Instance = null;
		}

		public bool GetUp(Vector3 i_point, out Vector3 o_up)
		{
			Vector3 gravity = GetGravity(i_point);
			o_up = -gravity.normalized;
			return gravity.sqrMagnitude > 0.001f;
		}

		public Vector3 GetGravity(Vector3 i_point)
		{
			Vector3 totalGravity = Vector3.zero;
			foreach (var pp in m_planetProviders)
			{
				totalGravity += pp.GetGravity(i_point);
			}
			return totalGravity;
		}

		public float GetGravityProminence(Vector3 i_point)
		{
			float totalGravity = 0.0f;
			foreach (var pp in m_planetProviders)
			{
				totalGravity += pp.GetGravityProminence(i_point);
			}
			return totalGravity;
		}

		public IGravityProvider GetMostProminent(Vector3 i_point)
		{
			float prominenceRecord = 0.0f;
			IGravityProvider mostProminent = null;
			foreach (var pp in m_planetProviders)
			{
				float prominence = pp.GetGravityProminence(i_point);
				if (prominence >= prominenceRecord)
				{
					prominenceRecord = prominence;
					mostProminent = pp;
				}
			}
			return mostProminent;
		}

		private List<IGravityProvider> m_planetProviders = new List<IGravityProvider>();
	}
}