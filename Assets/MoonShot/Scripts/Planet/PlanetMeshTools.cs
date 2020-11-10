using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Moonshot.Planet
{
	public static class PlanetMeshTools
	{
		public interface IMeshBuildContext
		{
			void NudgeDirection(ref Vector3 io_direction);
			float GetRadiusInDirection(Vector3 i_direction);
			float SmoothNormals(Vector3 i_direction);
		}

		public class MeshBuilder
		{
			public List<Vector3> Vertices = new List<Vector3>();
			public List<Vector3> Normals = new List<Vector3>();
			public List<int> Triangles = new List<int>();

			public Mesh BuildMesh(string i_name)
			{
				var mesh = new Mesh();
				mesh.name = i_name;

				mesh.subMeshCount = 1;
				mesh.SetVertices(Vertices.ToArray());
				mesh.SetNormals(Normals.ToArray());
				//mesh.SetIndices(Triangles.ToArray(), MeshTopology.Triangles, 0);
				mesh.SetTriangles(Triangles.ToArray(), 0);

				//mesh.RecalculateNormals();
				mesh.RecalculateTangents();
				mesh.RecalculateBounds();
				//mesh.Optimize();
				mesh.UploadMeshData(false);
				return mesh;
			}
		}

		public static Mesh BuildFullPlanetMesh(IMeshBuildContext i_context, int i_subdivs = 1, string i_name = "Built Full Planet Mesh")
		{
			var mb = new MeshBuilder();

			for (int i = 0; i < Icosahedron.Triangles.Length; ++i)
			{
				PopulateMeshForIcoFace(mb, Icosahedron.Triangles[i], i_context, i_subdivs);
			}

			return mb.BuildMesh(i_name);
		}

		private static void PopulateMeshForIcoFace(MeshBuilder mb, IcosahedronDefBuilder.Triangle triangle, IMeshBuildContext i_context, int i_subdivs)
		{
			if (i_subdivs > 1)
			{
				IcosahedronDefBuilder.Triangle t0, t1, t2, t3;
				triangle.Subdivide(out t0, out t1, out t2, out t3);
				PopulateMeshForIcoFace(mb, t0, i_context, i_subdivs - 1);
				PopulateMeshForIcoFace(mb, t1, i_context, i_subdivs - 1);
				PopulateMeshForIcoFace(mb, t2, i_context, i_subdivs - 1);
				PopulateMeshForIcoFace(mb, t3, i_context, i_subdivs - 1);
			}
			else
			{
				AddTriangleToMesh(mb, i_context, TransformPoint(triangle.v0, i_context), TransformPoint(triangle.v1, i_context), TransformPoint(triangle.v2, i_context));
			}
		}

		private static void AddTriangleToMesh(MeshBuilder mb, IMeshBuildContext i_context, Vector3 v0, Vector3 v1, Vector3 v2)
		{
			Vector3 triNormal = GetTriNormal(v0, v1, v2);
			mb.Triangles.Add(mb.Vertices.Count);
			mb.Vertices.Add(v0);
			mb.Normals.Add(GetNormal(i_context, v0, triNormal));
			mb.Triangles.Add(mb.Vertices.Count);
			mb.Vertices.Add(v1);
			mb.Normals.Add(GetNormal(i_context, v1, triNormal));
			mb.Triangles.Add(mb.Vertices.Count);
			mb.Vertices.Add(v2);
			mb.Normals.Add(GetNormal(i_context, v2, triNormal));
		}

		private static Vector3 GetNormal(IMeshBuildContext i_context, Vector3 v0, Vector3 triNormal)
		{
			Vector3 direction = v0.normalized;
			return Vector3.Lerp(triNormal, direction, i_context.SmoothNormals(direction));
		}

		private static Vector3 GetTriNormal(Vector3 v0, Vector3 v1, Vector3 v2)
		{
			return Vector3.Cross(v1 - v0, v2 - v0).normalized;
		}

		private static Vector3 TransformPoint(Vector3 p, IMeshBuildContext i_context)
		{
			Vector3 direction = p.normalized;
			i_context.NudgeDirection(ref direction);
			return i_context.GetRadiusInDirection(direction) * direction;
		}
	}
}