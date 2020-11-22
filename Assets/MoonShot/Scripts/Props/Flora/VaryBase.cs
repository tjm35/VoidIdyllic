using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public abstract class VaryBase : MonoBehaviour
	{
		public int m_additionalSeed = 0;

		public abstract void ApplyVariation();
		public virtual void BakeMesh() { }
		public virtual void UnbakeMesh() { }

		[Button]
		public void RerollAdditionalSeedAndVary()
		{
			m_additionalSeed = Random.Range(int.MinValue, int.MaxValue);
			ApplyVariation();
		}

		protected static Vector3 RandEuler(System.Random i_rand, Vector3 i_angleRange)
		{
			return new Vector3(Rand(i_rand, -i_angleRange.x, i_angleRange.x),Rand(i_rand, -i_angleRange.y, i_angleRange.y),Rand(i_rand, -i_angleRange.z, i_angleRange.z));
		}

		protected static Vector3 RandInBox(System.Random i_rand, Vector3 i_halfSize)
		{
			return new Vector3(Rand(i_rand, -i_halfSize.x, i_halfSize.x),Rand(i_rand, -i_halfSize.y, i_halfSize.y),Rand(i_rand, -i_halfSize.z, i_halfSize.z));
		}

		protected static float Rand(System.Random i_rand, float i_min, float i_max)
		{
			return ((float)i_rand.NextDouble() * (i_max - i_min)) + i_min;
		}

		protected static int RandInt(System.Random i_rand, int i_min, int i_max)
		{
			return i_rand.Next(i_min, i_max);
		}

		protected System.Random MakeRandomiser()
		{
			int seedFromPos = transform.position.GetHashCode();
			int seed = seedFromPos ^ m_additionalSeed;
			return new System.Random(seed);
		}

	}

}