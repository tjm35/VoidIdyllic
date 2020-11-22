using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class VaryFlora : MonoBehaviour
	{
		[Required]
		public SkinnedMeshRenderer m_source;
		public int m_additionalSeed = 0;
		public Vector3 m_angleRange = new Vector3(10.0f, 5.0f, 10.0f);

		[Button]
		public void ApplyVariation()
		{
			UnbakeMesh();

			var rand = MakeRandomiser();

			var smr = m_source;

			for (int i = 0; i < smr.bones.Length; ++i)
			{
				var bone = smr.bones[i];
				var constraint = bone.GetComponent<VaryConstraint>();
				if (constraint)
				{
					Vector3 ranges = m_angleRange;
					if (constraint.m_useCustomAngleRange)
					{
						ranges = constraint.m_customAngleRange;
					}
					var anglesRotation = Quaternion.Euler(RandEuler(rand, ranges));
					bone.localRotation = anglesRotation * constraint.m_baseRotation;

					var globalLook = transform.TransformDirection(constraint.m_forcedFacingDirection);
					var forwardLookGlobalRotation = Quaternion.LookRotation(globalLook, bone.up);
					var axisLookGlobalRotation = forwardLookGlobalRotation * Quaternion.FromToRotation(constraint.m_forcedFacingAxis, Vector3.forward);

					bone.rotation = Quaternion.Lerp(bone.rotation, axisLookGlobalRotation, constraint.m_forcedFacingWeight);
				}
				else
				{
					bone.localRotation = Quaternion.Euler(RandEuler(rand, m_angleRange));
				}
			}
		}

		[Button]
		public void BakeMesh()
		{
			Mesh m = new Mesh();

			var smr = m_source;
			smr.BakeMesh(m);
			m.name = "Baked " + smr.gameObject.name;
			smr.gameObject.SetActive(false);

			var mf = GetComponent<MeshFilter>();
			if (mf == null)
			{
				mf = gameObject.AddComponent<MeshFilter>();
			}

			mf.sharedMesh = m;

			var mr = GetComponent<MeshRenderer>();
			if (mr == null)
			{
				mr = gameObject.AddComponent<MeshRenderer>();
			}

			mr.enabled = true;
			mr.sharedMaterials = smr.sharedMaterials;
		}

		[Button]
		public void UnbakeMesh()
		{
			var mr = GetComponent<MeshRenderer>();
			if (mr && mr.enabled)
			{
				mr.enabled = false;

				var smr = m_source;
				smr.gameObject.SetActive(true);
			}
		}

		[Button]
		public void RerollAdditionalSeedAndVary()
		{
			m_additionalSeed = Random.Range(int.MinValue, int.MaxValue);
			ApplyVariation();
		}

		private static Vector3 RandEuler(System.Random i_rand, Vector3 i_angleRange)
		{
			return new Vector3(Rand(i_rand, -i_angleRange.x, i_angleRange.x),Rand(i_rand, -i_angleRange.y, i_angleRange.y),Rand(i_rand, -i_angleRange.z, i_angleRange.z));
		}

		private static float Rand(System.Random i_rand, float i_min, float i_max)
		{
			return ((float)i_rand.NextDouble() * (i_max - i_min)) + i_min;
		}

		private System.Random MakeRandomiser()
		{
			int seedFromPos = transform.position.GetHashCode();
			int seed = seedFromPos ^ m_additionalSeed;
			return new System.Random(seed);
		}

	}
}