using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UnityEngine;

namespace Moonshot.Planet
{
	public class IcosahedronDefBuilder : MonoBehaviour
	{
		public Mesh m_icosahedronMesh;
		public string m_writePath = "Assets/Moonshot/Scripts/Planet/Icosahedron.cs";

		public struct Triangle
		{
			public Vector3 v0;
			public Vector3 v1;
			public Vector3 v2;

			public void Subdivide(out Triangle t0, out Triangle t1, out Triangle t2, out Triangle t3)
			{
				Vector3 m0 = 0.5f * (v0 + v1);
				Vector3 m1 = 0.5f * (v1 + v2);
				Vector3 m2 = 0.5f * (v2 + v0);

				t0 = new Triangle { v0 = v0, v1 = m0, v2 = m2 };
				t1 = new Triangle { v0 = v1, v1 = m1, v2 = m0 };
				t2 = new Triangle { v0 = v2, v1 = m2, v2 = m1 };
				t3 = new Triangle { v0 = m0, v1 = m1, v2 = m2 };
			}
		}

		[ContextMenu("Build Def")]
		private void BuildDef()
		{
			Debug.Log($"IcosahedronDefBuilder: Mesh has {m_icosahedronMesh.subMeshCount} submeshes.");
			Debug.Assert(m_icosahedronMesh.subMeshCount > 0);
			Debug.Log($"IcosahedronDefBuilder: Submesh 0 is {m_icosahedronMesh.GetTopology(0)}");
			Debug.Assert(m_icosahedronMesh.GetTopology(0) == MeshTopology.Triangles);

			var triangles = m_icosahedronMesh.GetTriangles(0);
			int triCount = m_icosahedronMesh.triangles.Length / 3;
			Debug.Log($"IcosahedronDefBuilder: Mesh has {m_icosahedronMesh.triangles.Length / 3} triangles.");
			var indices = m_icosahedronMesh.GetIndices(0);
			var vertices = m_icosahedronMesh.vertices;

			var tris = new Triangle[triCount];

			if (indices != null)
			{
				for (int i = 0; i < triCount; ++i)
				{
					tris[i].v0 = vertices[indices[(i * 3) + 0]];
					tris[i].v1 = vertices[indices[(i * 3) + 1]];
					tris[i].v2 = vertices[indices[(i * 3) + 2]];
				}
			}
			else
			{
				for (int i = 0; i < triCount; ++i)
				{
					tris[i].v0 = vertices[triangles[(i * 3) + 0]];
					tris[i].v1 = vertices[triangles[(i * 3) + 1]];
					tris[i].v2 = vertices[triangles[(i * 3) + 2]];
				}
			}

			var sb = new StringBuilder();
			sb.AppendLine("using UnityEngine;");
			sb.AppendLine("");
			sb.AppendLine("namespace Moonshot.Planet");
			sb.AppendLine("{");
			sb.AppendLine("	public static class Icosahedron");
			sb.AppendLine("	{");
			sb.AppendLine("		public static IcosahedronDefBuilder.Triangle[] Triangles = new IcosahedronDefBuilder.Triangle[]");
			sb.AppendLine("		{");
			for (int i = 0; i < triCount; ++i)
			{
				sb.AppendLine($"			{TriangleDef(tris[i])},");
			}
			sb.AppendLine("		};");
			sb.AppendLine("	}");
			sb.AppendLine("}");

			File.WriteAllText(m_writePath, sb.ToString());
		}

		private string VectorDef(Vector3 i_vec)
		{
			return $"new Vector3((float){i_vec.x}, (float){i_vec.y}, (float){i_vec.z})";
		}

		private string TriangleDef(Triangle i_tri)
		{
			return $"new IcosahedronDefBuilder.Triangle {{ v0 = {VectorDef(i_tri.v0)}, v1 = {VectorDef(i_tri.v1)}, v2 = {VectorDef(i_tri.v2)} }}";
		}
	}
}