using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOVSetting : MonoBehaviour
{
	public float FOV
	{
		get { return s_FOV; }
		set { s_FOV = value; }
	}
	public static float s_FOV = 60.0f;
}
