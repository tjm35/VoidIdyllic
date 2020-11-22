using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class MassVary : MonoBehaviour
	{
		[Button]
		public void ApplyVariationToAll()
		{
			foreach (var vb in transform.GetComponentsInDescendents<VaryBase>())
			{
				vb.ApplyVariation();
			}
		}

		[Button]
		public void BakeMeshOnAll()
		{
			foreach (var vb in transform.GetComponentsInDescendents<VaryBase>())
			{
				vb.BakeMesh();
			}
		}

		[Button]
		public void UnbakeMeshOnAll()
		{
			foreach (var vb in transform.GetComponentsInDescendents<VaryBase>())
			{
				vb.UnbakeMesh();
			}
		}


	}
}