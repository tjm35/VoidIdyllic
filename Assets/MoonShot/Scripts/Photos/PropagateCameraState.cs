using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropagateCameraState : MonoBehaviour
{
	[ReorderableList,Required]
	public List<Camera> m_targets;

	public float FocalLength { get; set; } = 20;
	public float LensShiftX { get; set; } = 0;
	public float LensShiftY { get; set; } = 0;

    void LateUpdate()
    {
        foreach (var target in m_targets)
		{
			target.focalLength = FocalLength;
			target.lensShift = new Vector2(LensShiftX, LensShiftY);
		}
    }
}
