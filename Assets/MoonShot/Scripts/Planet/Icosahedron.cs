using UnityEngine;

namespace Moonshot.Planet
{
	public static class Icosahedron
	{
		public static IcosahedronDefBuilder.Triangle[] Triangles = new IcosahedronDefBuilder.Triangle[]
		{
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0, (float)-1, (float)0), v1 = new Vector3((float)0.276385, (float)-0.447215, (float)0.85064), v2 = new Vector3((float)-0.7236, (float)-0.447215, (float)0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.7236, (float)-0.447215, (float)0.52572), v1 = new Vector3((float)-0.7236, (float)-0.447215, (float)-0.52572), v2 = new Vector3((float)0, (float)-1, (float)0) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0, (float)-1, (float)0), v1 = new Vector3((float)0.894425, (float)-0.447215, (float)0), v2 = new Vector3((float)0.276385, (float)-0.447215, (float)0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0, (float)-1, (float)0), v1 = new Vector3((float)0.276385, (float)-0.447215, (float)-0.85064), v2 = new Vector3((float)0.894425, (float)-0.447215, (float)0) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0, (float)-1, (float)0), v1 = new Vector3((float)-0.7236, (float)-0.447215, (float)-0.52572), v2 = new Vector3((float)0.276385, (float)-0.447215, (float)-0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.7236, (float)-0.447215, (float)0.52572), v1 = new Vector3((float)-0.894425, (float)0.447215, (float)0), v2 = new Vector3((float)-0.7236, (float)-0.447215, (float)-0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.276385, (float)-0.447215, (float)0.85064), v1 = new Vector3((float)-0.276385, (float)0.447215, (float)0.85064), v2 = new Vector3((float)-0.7236, (float)-0.447215, (float)0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.894425, (float)-0.447215, (float)0), v1 = new Vector3((float)0.7236, (float)0.447215, (float)0.52572), v2 = new Vector3((float)0.276385, (float)-0.447215, (float)0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.276385, (float)-0.447215, (float)-0.85064), v1 = new Vector3((float)0.7236, (float)0.447215, (float)-0.52572), v2 = new Vector3((float)0.894425, (float)-0.447215, (float)0) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.7236, (float)-0.447215, (float)-0.52572), v1 = new Vector3((float)-0.276385, (float)0.447215, (float)-0.85064), v2 = new Vector3((float)0.276385, (float)-0.447215, (float)-0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.7236, (float)-0.447215, (float)0.52572), v1 = new Vector3((float)-0.276385, (float)0.447215, (float)0.85064), v2 = new Vector3((float)-0.894425, (float)0.447215, (float)0) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.276385, (float)-0.447215, (float)0.85064), v1 = new Vector3((float)0.7236, (float)0.447215, (float)0.52572), v2 = new Vector3((float)-0.276385, (float)0.447215, (float)0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.894425, (float)-0.447215, (float)0), v1 = new Vector3((float)0.7236, (float)0.447215, (float)-0.52572), v2 = new Vector3((float)0.7236, (float)0.447215, (float)0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.276385, (float)-0.447215, (float)-0.85064), v1 = new Vector3((float)-0.276385, (float)0.447215, (float)-0.85064), v2 = new Vector3((float)0.7236, (float)0.447215, (float)-0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.7236, (float)-0.447215, (float)-0.52572), v1 = new Vector3((float)-0.894425, (float)0.447215, (float)0), v2 = new Vector3((float)-0.276385, (float)0.447215, (float)-0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.276385, (float)0.447215, (float)0.85064), v1 = new Vector3((float)0, (float)1, (float)0), v2 = new Vector3((float)-0.894425, (float)0.447215, (float)0) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.7236, (float)0.447215, (float)0.52572), v1 = new Vector3((float)0, (float)1, (float)0), v2 = new Vector3((float)-0.276385, (float)0.447215, (float)0.85064) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)0.7236, (float)0.447215, (float)-0.52572), v1 = new Vector3((float)0, (float)1, (float)0), v2 = new Vector3((float)0.7236, (float)0.447215, (float)0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.276385, (float)0.447215, (float)-0.85064), v1 = new Vector3((float)0, (float)1, (float)0), v2 = new Vector3((float)0.7236, (float)0.447215, (float)-0.52572) },
			new IcosahedronDefBuilder.Triangle { v0 = new Vector3((float)-0.894425, (float)0.447215, (float)0), v1 = new Vector3((float)0, (float)1, (float)0), v2 = new Vector3((float)-0.276385, (float)0.447215, (float)-0.85064) },
		};
	}
}
