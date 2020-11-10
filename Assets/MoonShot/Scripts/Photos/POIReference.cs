using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Photos
{
	public class POIReference : MonoBehaviour, IPOIContext
	{
		public IPOIContext m_referencedPOI;

		public void SetupMaterial(MaterialPropertyBlock i_block)
		{
			m_referencedPOI?.SetupMaterial(i_block);
		}
	}
}