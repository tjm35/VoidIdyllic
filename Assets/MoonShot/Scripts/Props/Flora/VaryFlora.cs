using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Props
{
	public class VaryFlora : VaryBase
	{
		[Required]
		public SkinnedMeshRenderer m_source;
		public Vector3 m_angleRange = new Vector3(10.0f, 5.0f, 10.0f);
		public float m_maxValidTwist = 30.0f;
		public bool m_forceRerollIfInvalid = true;
		public int m_maxRerolls = 3;

		[Button]
		public override void ApplyVariation()
		{
			UnbakeMesh();

			int rolls = 0;
			bool needsRevary = true;

			while (rolls < m_maxRerolls && needsRevary)
			{
				if (rolls > 0)
				{
					m_additionalSeed = Random.Range(int.MinValue, int.MaxValue);
				}
				rolls++;
				needsRevary = false;
				
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

						float twistMag = Mathf.Abs(Mathf.DeltaAngle(0.0f, bone.localEulerAngles.y));
						if (twistMag > m_maxValidTwist && !constraint.m_ignoreTwistLimits)
						{
							Debug.Log($"Twist limit exceeded on {bone.gameObject.name} ({twistMag} > {m_maxValidTwist}).");
							if (m_forceRerollIfInvalid)
							{
								needsRevary = true;
							}
						}
					}
					else
					{
						bone.localRotation = Quaternion.Euler(RandEuler(rand, m_angleRange));
					}
				}
			}
		}

		[Button]
		public override void BakeMesh()
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
		public override void UnbakeMesh()
		{
			var mr = GetComponent<MeshRenderer>();
			if (mr)
			{
				mr.enabled = false;
			}
			var smr = m_source;
			smr.gameObject.SetActive(true);
		}

	}
}