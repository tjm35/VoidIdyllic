using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Moonshot.Planet
{
	public static class CubemapSampler
	{
		public static Color SampleCubemap(Cubemap i_map, Vector3 i_normal)
		{
			// For now just load a single pixel.
			GetNearestPixel(i_map, i_normal, out CubemapFace face, out int x, out int y);
			var col = LoadPixel(i_map, face, x, y);
			return col;
		}

		private static Color LoadPixel(Cubemap i_map, CubemapFace face, int x, int y)
		{
			return i_map.GetPixel(face, x, y);
		}

		private static void GetNearestPixel(Cubemap i_map, Vector3 i_normal, out CubemapFace face, out int x, out int y)
		{
			float[] bestAddress = new float[3] { 0, 0, 0 };
			int bestFace = -1;

			for (int f = 0; f < 6; ++f)
			{
				float forward = Vector3.Dot(i_normal, s_faceAxes[f, 2]);
				if (forward > bestAddress[2])
				{
					bestAddress[0] = Vector3.Dot(i_normal, s_faceAxes[f, 0]) / forward;
					bestAddress[1] = Vector3.Dot(i_normal, s_faceAxes[f, 1]) / forward;
					bestAddress[2] = forward;
					bestFace = f;
				}
			}

			face = (CubemapFace)bestFace;
			x = (int)((0.5f + bestAddress[0]) * i_map.width);
			y = (int)((0.5f + bestAddress[1]) * i_map.width);
		}

		private static Vector3[,] s_faceAxes = new Vector3[,] {
			{ -Vector3.forward, -Vector3.up, Vector3.right },
			{ Vector3.forward, -Vector3.up, -Vector3.right },
			{ Vector3.right, Vector3.forward, Vector3.up },
			{ Vector3.right, -Vector3.forward, -Vector3.up },
			{ Vector3.right, -Vector3.up, Vector3.forward },
			{ -Vector3.right, -Vector3.up, -Vector3.forward }
		};
	}
}