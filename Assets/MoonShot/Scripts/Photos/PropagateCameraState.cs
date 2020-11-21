using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PropagateCameraState : MonoBehaviour
{
	[ReorderableList]
	public List<Camera> m_targets;
	[Required]
	public Volume m_cameraProcessingVolume;

	public float FocalLength { get; set; } = 20;
	public float LensShiftX { get; set; } = 0;
	public float LensShiftY { get; set; } = 0;
	public float AperturePower { get; set; } = 0;
	public float ISOPower { get; set; } = 0;
	public float Temperature { get; set; } = 0;

	void LateUpdate()
	{
		foreach (var target in m_targets)
		{
			target.focalLength = FocalLength;
			target.lensShift = new Vector2(LensShiftX, LensShiftY);
		}

		var profile = m_cameraProcessingVolume.profile;

		{
			if (!profile.TryGet<DepthOfField>(out var dof))
			{
				dof = profile.Add<DepthOfField>(false);
			}
			dof.active = true;
			dof.aperture.Override(5.6f * Mathf.Pow(2.0f, AperturePower));
			dof.focalLength.Override(FocalLength * 3.0f);
		}

		{
			if (!profile.TryGet<ColorAdjustments>(out var adjust))
			{
				adjust = profile.Add<ColorAdjustments>(false);
			}
			adjust.active = true;
			adjust.postExposure.Override((2.0f * AperturePower) + ISOPower);
		}

		{
			if (!profile.TryGet<WhiteBalance>(out var white))
			{
				white = profile.Add<WhiteBalance>(false);
			}
			white.active = true;
			white.temperature.Override(Temperature);
		}

		m_cameraProcessingVolume.profile = profile;
}
}
