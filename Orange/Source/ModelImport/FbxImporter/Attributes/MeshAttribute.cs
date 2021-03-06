using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using Lime;

namespace Orange.FbxImporter
{
	public class Bone
	{
		public Matrix44 Offset { get; set; }

		public string Name { get; set; }
	}

	public class MeshAttribute : NodeAttribute
	{
		public List<Submesh> Submeshes { get; private set; } = new List<Submesh>();

		public override FbxNodeType Type { get; } = FbxNodeType.MESH;

		public MeshAttribute() : base(IntPtr.Zero)
		{
		}

		public static MeshAttribute FromSubmesh(IntPtr submeshPtr)
		{
			return new MeshAttribute {
				Submeshes = { new Submesh(submeshPtr) }
			};
		}

		public static MeshAttribute Combine(MeshAttribute meshAttribute1, MeshAttribute meshAttribute2)
		{
			var sm = new List<Submesh>();
			sm.AddRange(meshAttribute1.Submeshes);
			sm.AddRange(meshAttribute2.Submeshes);
			return new MeshAttribute {
				Submeshes = sm
			};
		}
	}

	public class Submesh : NodeAttribute
	{
		public int[] Indices { get; set; }

		public int MaterialIndex { get; set; }

		public Mesh3D.Vertex[] Vertices { get; set; }

		public Bone[] Bones { get; set; }

		public Submesh(IntPtr ptr) : base(ptr)
		{
			var native = FbxNodeGetMeshAttribute(NativePtr, true);
			if (native == IntPtr.Zero) {
				throw new FbxAtributeImportException(Type);
			}
			var mesh = native.ToStruct<MeshData>();
			var colors = mesh.colors.ToStructArray<Vec4>(mesh.verticesCount);
			var verices = mesh.points.ToStructArray<Vec3>(mesh.verticesCount);

			var uv = mesh.uvCoords.ToStructArray<Vec2>(mesh.verticesCount);
			var weights = mesh.weights.ToStructArray<WeightData>(mesh.verticesCount);
			var bones = mesh.bones.FromArrayOfPointersToStructArrayUnsafe<BoneData>(mesh.boneCount);
			var normals = mesh.normals.ToStructArray<Vec3>(mesh.verticesCount);

			Indices = mesh.vertices.ToIntArray(mesh.verticesCount);
			MaterialIndex = mesh.materialIndex;
			Vertices = new Mesh3D.Vertex[mesh.verticesCount];
			Bones = new Bone[mesh.boneCount];

			for (int i = 0; i < mesh.boneCount; i++) {
				Bones[i] = new Bone();
				Bones[i].Name = bones[i].name;
				Bones[i].Offset = bones[i].offset.ToStruct<Mat4x4>().ToLime();
			}

			for (var i = 0; i < mesh.verticesCount; i++) {
				var vertex =
				Vertices[i].Pos = verices[i].toLime();
				Vertices[i].Color = colors != null ? colors[i].toLimeColor() : Color4.White;
				Vertices[i].UV1 = uv[i].toLime();
				Vertices[i].Normal = normals[i].toLime();

				byte index;
				float weight;

				for (int j = 0; j < ImportConfig.BoneLimit; j++) {
					if (weights[i].Weights[j] != -1) {
						index = weights[i].Indices[j];
						weight = weights[i].Weights[j];
						switch (j) {
							case 0:
								Vertices[i].BlendIndices.Index0 = index;
								Vertices[i].BlendWeights.Weight0 = weight;
								break;
							case 1:
								Vertices[i].BlendIndices.Index1 = index;
								Vertices[i].BlendWeights.Weight1 = weight;
								break;
							case 2:
								Vertices[i].BlendIndices.Index2 = index;
								Vertices[i].BlendWeights.Weight2 = weight;
								break;
							case 3:
								Vertices[i].BlendIndices.Index3 = index;
								Vertices[i].BlendWeights.Weight3 = weight;
								break;
							default:
								break;
						}
					}
				}
			}
		}

		#region Pinvokes

		[DllImport(ImportConfig.LibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr FbxNodeGetMeshAttribute(IntPtr node, bool IsLimitBoneWeights);

		[DllImport(ImportConfig.LibName, CallingConvention = CallingConvention.Cdecl)]
		public static extern IntPtr FbxNodeGetMeshMaterial(IntPtr pMesh, int idx);

		#endregion

		#region MarshalingStructures

		[StructLayout(LayoutKind.Sequential)]
		private class MeshData
		{
			public IntPtr vertices;

			public IntPtr points;

			public IntPtr colors;

			public IntPtr uvCoords;

			public IntPtr normals;

			public IntPtr weights;

			public IntPtr bones;

			[MarshalAs(UnmanagedType.I4)]
			public int materialIndex;

			[MarshalAs(UnmanagedType.I4)]
			public int verticesCount;

			[MarshalAs(UnmanagedType.I4)]
			public int boneCount;
		}

		[StructLayout(LayoutKind.Sequential, CharSet = ImportConfig.Charset)]
		private class BoneData
		{
			public string name;

			public IntPtr offset;
		}

		[StructLayout(LayoutKind.Sequential)]
		private class WeightData
		{
			[MarshalAs(UnmanagedType.ByValArray, SizeConst = ImportConfig.BoneLimit)]
			public byte[] Indices;

			[MarshalAs(UnmanagedType.ByValArray, SizeConst = ImportConfig.BoneLimit)]
			public float[] Weights;
		}

		#endregion
	}
}
