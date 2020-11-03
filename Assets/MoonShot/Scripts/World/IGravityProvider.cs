using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.World
{
	public interface IGravityProvider
	{
		bool GetUp(Vector3 i_point, out Vector3 o_up);
		Vector3 GetGravity(Vector3 i_point);
	}
}